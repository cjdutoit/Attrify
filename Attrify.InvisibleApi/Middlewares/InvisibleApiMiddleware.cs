// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Attrify.Attributes;
using Attrify.InvisibleApi.Models;
using Microsoft.AspNetCore.Http;

namespace Attrify.Middlewares
{
    public class InvisibleApiMiddleware
    {
        private readonly RequestDelegate next;
        private readonly InvisibleApiKey visibilityHeader;

        public InvisibleApiMiddleware(RequestDelegate next, InvisibleApiKey visibilityHeader)
        {
            this.next = next;
            this.visibilityHeader = visibilityHeader;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint != null && IsInvisibleApi(endpoint, context))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            await next(context);
        }

        private bool IsInvisibleApi(Endpoint endpoint, HttpContext context)
        {
            if (!endpoint.Metadata.OfType<InvisibleApiAttribute>().Any())
            {
                return false;
            }

            bool isVisibilityHeaderValid =
                context.Request.Headers.TryGetValue(visibilityHeader.Key, out var headerValue)
                    && headerValue == visibilityHeader.Value;

            bool isUserAuthenticatedAndInRole = context.User.Identity?.IsAuthenticated == true
                && context.User.IsInRole(visibilityHeader.Key);

            if (isVisibilityHeaderValid && isUserAuthenticatedAndInRole)
            {
                return false;
            }

            return true;
        }
    }
}
