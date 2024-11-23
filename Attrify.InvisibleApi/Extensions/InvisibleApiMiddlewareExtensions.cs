// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using Attrify.InvisibleApi.Models;
using Attrify.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Attrify.Extensions
{
    public static class InvisibleApiMiddlewareExtensions
    {
        /// <summary>
        /// Adds the <see cref="InvisibleApiMiddleware"/> to the application's middleware pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <param name="invisibleApiKey">
        /// An instance of <see cref="InvisibleApiKey"/> representing the header key and value for visibility.
        /// </param>
        /// <returns>The modified <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseInvisibleApiMiddleware(
            this IApplicationBuilder builder,
            InvisibleApiKey invisibleApiKey)
        {
            // Use the InvisibleApiMiddleware with the converted key
            return builder.UseMiddleware<InvisibleApiMiddleware>(invisibleApiKey);
        }
    }
}
