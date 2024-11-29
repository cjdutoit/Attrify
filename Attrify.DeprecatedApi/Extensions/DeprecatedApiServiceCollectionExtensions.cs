// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using Attrify.DeprecatedApi.Brokers.DateTimes;
using Attrify.DeprecatedApi.Controllers;
using Attrify.DeprecatedApi.Services.DeprecatedApiScanners;
using Microsoft.Extensions.DependencyInjection;

namespace Attrify.Extensions
{
    /// <summary>
    /// Provides extension methods for registering deprecated API support in the application.
    /// </summary>
    public static class DeprecatedApiServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the necessary services and controllers to support deprecated API functionality.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddDeprecationStatusController(this IServiceCollection services)
        {
            services.AddSingleton<IDateTimeBroker, DateTimeBroker>();
            services.AddSingleton<IDeprecatedApiScannerService, DeprecatedApiScannerService>();
            services.AddControllers().AddApplicationPart(typeof(DeprecationStatusController).Assembly);

            return services;
        }
    }
}
