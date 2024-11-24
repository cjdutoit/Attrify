// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Attrify.WebApp.Tests.Acceptance
{
    public class TestAuthHandler : AuthenticationHandler<CustomAuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<CustomAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string randomOidGuid = Guid.NewGuid().ToString();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Role, "Administrators"),
                new Claim("oid", randomOidGuid)
            };

            var invisibleApiKey = Options.InvisibleApiKey;
            if (invisibleApiKey != null && !string.IsNullOrWhiteSpace(invisibleApiKey.Key))
            {
                claims.Add(new Claim(ClaimTypes.Role, invisibleApiKey.Key));
            }

            var identity = new ClaimsIdentity(claims, "TestScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
