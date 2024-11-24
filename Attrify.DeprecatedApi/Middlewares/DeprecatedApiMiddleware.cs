// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;
using System.Threading.Tasks;
using Attrify.Attributes;
using Microsoft.AspNetCore.Http;

namespace Attrify.Middlewares
{
    public class DeprecatedApiMiddleware
    {
        private readonly RequestDelegate next;

        public DeprecatedApiMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var deprecatedApiAttribute = endpoint?.Metadata.GetMetadata<DeprecatedApiAttribute>();

            if (deprecatedApiAttribute != null)
            {
                if (!context.Response.Headers.ContainsKey("Sunset"))
                {
                    context.Response.Headers.Add("Sunset", deprecatedApiAttribute.Sunset);
                }

                if (!string.IsNullOrEmpty(deprecatedApiAttribute.Warning))
                {
                    if (!context.Response.Headers.ContainsKey("Warning"))
                    {
                        context.Response.Headers.Add("Warning", $"{deprecatedApiAttribute.Warning}");
                    }
                }

                if (!string.IsNullOrEmpty(deprecatedApiAttribute.Link))
                {
                    if (!context.Response.Headers.ContainsKey("Link"))
                    {
                        context.Response.Headers.Add("Link", $"<{deprecatedApiAttribute.Link}>; rel=\"deprecation\"");
                    }
                }

                bool isParsed = DateTime.TryParse(deprecatedApiAttribute.Sunset, out DateTime sunsetDate);

                if (!isParsed)
                {
                    sunsetDate = DateTime.UtcNow;
                }

                if (sunsetDate <= System.DateTime.UtcNow)
                {
                    context.Response.StatusCode = StatusCodes.Status410Gone;

                    var errorDetails = new
                    {
                        StatusCode = StatusCodes.Status410Gone,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.9",
                        Title = "Deprecated API",

                        Error =
                            $"This API has been sunset and is no longer available.  " +
                            $"The link should provide details about alternatives, or migration steps.",

                        SunsetDate = sunsetDate.ToString("yyyy-MM-dd"),
                        Link = deprecatedApiAttribute.Link ?? "N/A"
                    };

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(errorDetails);

                    return;
                }
            }

            await next(context);
        }
    }
}
