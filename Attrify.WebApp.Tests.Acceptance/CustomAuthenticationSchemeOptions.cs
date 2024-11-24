// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using Attrify.InvisibleApi.Models;
using Microsoft.AspNetCore.Authentication;

namespace Attrify.WebApp.Tests.Acceptance
{
    public class CustomAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public InvisibleApiKey InvisibleApiKey { get; set; }
    }
}
