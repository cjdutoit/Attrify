// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using Attrify.DeprecatedApi.Services.DeprecatedApiScanners;
using Attrify.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Attrify.Extensions
{
    public static class DeprecatedApiMiddlewareExtensions
    {
        /// <summary>
        /// Adds middleware to handle API deprecation warnings and headers 
        /// based on the <see cref="DeprecatedApiAttribute"/>.
        /// 
        /// The middleware inspects the endpoint metadata for the attribute and:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///         Adds a <c>Sunset</c> HTTP header with the specified sunset date, 
        ///         indicating when the API will no longer be supported.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///         Adds a <c>Warning</c> HTTP header with a message informing clients of the deprecation.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///         Adds a <c>Link</c> HTTP header pointing to additional information about the deprecation timeline, 
        ///         alternatives, or migration steps.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///         Supports date-based logic, such as notifying clients how many days remain before the API is 
        ///         deprecated or if the sunset date has already passed.
        ///     </description>
        ///   </item>
        /// </list>
        /// This middleware enhances the API response to communicate deprecation details to clients in a standardized way.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <returns>The modified <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseDeprecatedApiMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DeprecatedApiMiddleware>();
        }

        /// <summary>
        /// Adds middleware to handle API deprecation warnings and headers 
        /// based on the <see cref="DeprecatedApiAttribute"/>.
        /// 
        /// The middleware inspects the endpoint metadata for the attribute and:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///         Adds a <c>Sunset</c> HTTP header with the specified sunset date, 
        ///         indicating when the API will no longer be supported.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///         Adds a <c>Warning</c> HTTP header with a message informing clients of the deprecation.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///         Adds a <c>Link</c> HTTP header pointing to additional information about the deprecation timeline, 
        ///         alternatives, or migration steps.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///         Supports date-based logic, such as notifying clients how many days remain before the API is 
        ///         deprecated or if the sunset date has already passed.
        ///     </description>
        ///   </item>
        /// </list>
        /// This middleware enhances the API response to communicate deprecation details to clients in a standardized way.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <returns>The modified <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseDeprecatedApiMiddlewareAndIncludeDeprecatedApisController(
            this IApplicationBuilder builder)
        {

            // Register DeprecatedApiScanner and service
            builder.ApplicationServices.GetRequiredService<IServiceCollection>()
                .AddSingleton<IDeprecatedApiScannerService, DeprecatedApiScannerService>();

            return builder.UseMiddleware<DeprecatedApiMiddleware>();
        }
    }
}
