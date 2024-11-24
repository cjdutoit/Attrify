// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

namespace Attrify.WebApplication.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string deprecatedApiRelativeBaseUrl = "api/deprecatedapi";
        public string NonDeprecatedRelativeUrl = $"{deprecatedApiRelativeBaseUrl}/active";
        public string PendingDeprecatedRelativeUrl = $"{deprecatedApiRelativeBaseUrl}/pending";
        public string DeprecatedRelativeUrl = $"{deprecatedApiRelativeBaseUrl}/deprecated";
        private const string invisibleApiRelativeUrl = "api/invisibleapi";
        public string VisibleRelativeUrl = $"{invisibleApiRelativeUrl}/visible";
        public string InvisibleRelativeUrl = $"{invisibleApiRelativeUrl}/invisible";
    }
}
