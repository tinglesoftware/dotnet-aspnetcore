﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Tingle.AspNetCore.Tokens.Binders;
using Tingle.AspNetCore.Tokens.Protection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions to the functionality <see cref="IMvcBuilder"/> for:
    /// <list type="number">
    /// <item><see cref="ContinuationToken{T}"/></item> and
    /// <item><see cref="TimedContinuationToken{T}"/></item>
    /// </list>
    /// </summary>
    public static class IMvcBuilderExtensions
    {
        /// <summary>
        /// Adds the services necessary for proper functionality of
        /// <see cref="ContinuationToken{T}"/> and
        /// <see cref="TimedContinuationToken{T}"/>.
        /// Ensure that data protection services are registered and that the backing store is sufficiently persistent
        /// By default calling services.AddControllers(...) registers data protection services.
        /// If the cancellation token is to be used for longer, the backing store needs to be persisted beyond an
        /// application reboot such as during an upgrade, memory pool recycle etc. Such a store can be a DbContext,
        /// Redis, Blob Storage, Windows Registry etc.
        /// </summary>
        /// <param name="builder">The application's MVC builder.</param>
        /// <param name="configure">
        /// An <see cref="Action{T}"/> to further configure instances of <see cref="TokenProtectorOptions"/>.
        /// </param>
        /// <returns>The modified builder.</returns>
        public static IMvcBuilder AddTokens(this IMvcBuilder builder, Action<TokenProtectorOptions> configure = null)
        {
            // Register the protector services
            var services = builder.Services;
            if (configure != null) services.Configure(configure);
            services.AddScoped(typeof(ITokenProtector<>), typeof(TokenProtector<>));

            return builder.AddMvcOptions(options =>
            {
                // Skip the binder registration if it was already added to the providers collection.
                if (!options.ModelBinderProviders.OfType<TokensModelBinderProvider>().Any())
                {
                    options.ModelBinderProviders.Insert(0, new TokensModelBinderProvider());
                }
            });
        }
    }
}