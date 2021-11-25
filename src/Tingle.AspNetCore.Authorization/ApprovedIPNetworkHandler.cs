﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Tingle.AspNetCore.Authorization;

/// <summary>
/// An <see cref="IAuthorizationHandler"/> that need is called to validate an instance of <see cref="ApprovedIPNetworkRequirement"/>
/// </summary>
public class ApprovedIPNetworkHandler : AuthorizationHandler<ApprovedIPNetworkRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogger logger;

    /// <summary>
    /// Creates an instance of <see cref="ApprovedIPNetworkHandler"/>
    /// </summary>
    /// <param name="httpContextAccessor">the accessor that provides the current http context</param>
    /// <param name="logger"></param>
    public ApprovedIPNetworkHandler(IHttpContextAccessor httpContextAccessor, ILogger<ApprovedIPNetworkHandler> logger)
    {
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Makes a decision if authorization is allowed based on a specific requirement.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The requirement to evaluate.</param>
    /// <returns></returns>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApprovedIPNetworkRequirement requirement)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var address = httpContext.Connection.RemoteIpAddress;
        logger.LogTrace("Checking approval for IP: '{0}'", address);
        if (address != null && requirement.IsApproved(address))
        {
            context.Succeed(requirement);
        }
        else
        {
            logger.LogWarning("Approval for IP: '{0}' failed", address);
        }

        return Task.CompletedTask;
    }
}
