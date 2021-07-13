﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Tingle.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for <see cref="IServiceCollection"/> related to authorization
    /// </summary>
    public static class AuthorizationPolicyBuilderExtensions
    {
        /// <summary>
        /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance.
        /// Ensure the necessary Authorization and framework services are added to the same collection
        /// using <c>services.AddApprovedNetworksHandler(...)</c>.
        /// </summary>
        /// <param name="builder">The instance to add to</param>
        /// <param name="networks">The allowed networks</param>
        public static AuthorizationPolicyBuilder RequireApprovedNetworks(this AuthorizationPolicyBuilder builder,
                                                                         IList<IPNetwork> networks)
        {
            // if there are no networks just return
            if (!networks.Any()) return builder;

            // reduce the networks where possible (referred to as supernetting)
            var reduced = IPNetwork.Supernet(networks.ToArray());

            // add the requirement
            return builder.AddRequirements(new ApprovedIPNetworkRequirement(reduced));
        }

        /// <summary>
        /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance.
        /// Ensure  the necessary Authorization and framework services are added to the same collection
        /// using <c>services.AddApprovedNetworksHandler(...)</c>.
        /// </summary>
        /// <param name="builder">The instance to add to</param>
        /// <param name="networks">The allowed networks</param>
        public static AuthorizationPolicyBuilder RequireApprovedNetworks(this AuthorizationPolicyBuilder builder,
                                                                         params string[] networks)
        {
            var parsed = networks?.Select(a => IPNetwork.Parse(a)).ToList();
            return builder.RequireApprovedNetworks(parsed);
        }

        /// <summary>
        /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance.
        /// Ensure the necessary Authorization and framework services are added to the same collection
        /// using <c>services.AddApprovedNetworksHandler(...)</c>.
        /// </summary>
        /// <param name="builder">The instance to add to</param>
        /// <param name="networks">The allowed networks</param>
        public static AuthorizationPolicyBuilder RequireApprovedNetworks(this AuthorizationPolicyBuilder builder,
                                                                         params IPNetwork[] networks)
        {
            return builder.RequireApprovedNetworks(networks.ToList());
        }

        /// <summary>
        /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance based on the configuration
        /// section provided. Ensure the necessary Authorization and framework services are added to the same
        /// collection using <c>services.AddApprovedNetworksHandler(...)</c>.
        /// </summary>
        /// <param name="builder">The instance to add to</param>
        /// <param name="section">
        /// The <see cref="IConfiguration"/> containing the values at the root.
        /// It must be bindable to an instanc of <c>List&lt;string&gt;</c>
        /// </param>
        /// <returns></returns>
        public static AuthorizationPolicyBuilder RequireApprovedNetworks(this AuthorizationPolicyBuilder builder, IConfiguration section)
        {
            var list = new List<string>();
            section.Bind(list);
            return builder.RequireApprovedNetworks(list.ToArray());
        }

        /// <summary>
        /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance, using Known Azure IPs.
        /// Ensure the necessary Authorization and framework services are added to the same collection
        /// using <c>services.AddApprovedNetworksHandler(...)</c>.
        /// Networks used are retrieved using <see cref="AzureIPNetworks.AzureIPsHelper"/>.
        /// </summary>
        /// <param name="builder">The instance to add to</param>
        /// <param name="serviceId">
        /// (Optional) The identifier of the service whose IP ranges to allow.
        /// When not provided(null), IPs from all services are added.
        /// </param>
        /// <param name="region">
        /// (Optional) The name of the region whose IP ranges to allow.
        /// When not provided(null), IPs from all regions are added.
        /// </param>
        public static AuthorizationPolicyBuilder RequireAzureIPNetworks(this AuthorizationPolicyBuilder builder,
                                                                        string serviceId = null,
                                                                        string region = null)
        {
            var ranges = AzureIPNetworks.AzureIPsHelper.GetAzureCloudIpsAsync().GetAwaiter().GetResult();
            var tags = ranges.Values.AsEnumerable();

            // if the service identifier is provided, only retain networks for that service
            if (serviceId != null) tags = tags.Where(t => t.Id == serviceId);

            // if the region is provided, only retain networks for that region
            if (region != null) tags = tags.Where(t => t.Properties.Region == region);

            // get the networks
            var networks = tags.SelectMany(t => t.Properties?.AddressPrefixes)
                               .Select(r => IPNetwork.Parse(r))
                               .ToArray();

            // create the requirement and add it to the builder
            return builder.RequireApprovedNetworks(networks);
        }

        /// <summary>
        /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance using IPs resolved via DNS.
        /// Ensure the necessary Authorization and framework services are added to the same collection
        /// using <c>services.AddApprovedNetworksHandler(...)</c>.
        /// </summary>
        /// <param name="builder">The instance to add to</param>
        /// <param name="fqdns">
        /// A list of Fully Qualified Domain Names.
        /// Each of them will be resolved to list of IP addresses using <see cref="Dns.GetHostAddresses(string)"/>
        /// </param>
        public static AuthorizationPolicyBuilder RequireNetworkFromDns(this AuthorizationPolicyBuilder builder,
                                                                       params string[] fqdns)
        {
            var networks = new List<IPNetwork>();

            // work on each FQDN
            foreach (var f in fqdns)
            {
                try
                {
                    // resolve the IP address from the hostname
                    var ips = Dns.GetHostAddresses(f);

                    // parse the IP addresses into IP networks
                    var rawNetworks = ips?.Select(ip => IPNetwork.Parse(ip.ToString(), CidrGuess.ClassLess)) ?? Array.Empty<IPNetwork>();

                    // add networks into the list if there are any
                    if (rawNetworks?.Any() ?? false)
                        networks.AddRange(rawNetworks);

                }
                catch (SocketException se) when (se.SocketErrorCode == SocketError.HostNotFound)
                {
                    continue;
                }
            }

            // if there are no networks, return
            if (!networks.Any()) return builder;

            // create the requirement and add it to the builder
            return builder.RequireApprovedNetworks(networks);
        }
    }
}
