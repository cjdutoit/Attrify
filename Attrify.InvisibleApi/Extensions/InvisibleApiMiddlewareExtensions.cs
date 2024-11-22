// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Collections.Generic;
using Attrify.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Attrify.Extensions
{
    public static class InvisibleApiMiddlewareExtensions
    {
        public static IApplicationBuilder UseInvisibleApiMiddleware(
            this IApplicationBuilder builder,
            KeyValuePair<string, string> visibilityHeader)
        {
            return builder.UseMiddleware<InvisibleApiMiddleware>(visibilityHeader);
        }
    }
}
