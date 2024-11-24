// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using Attrify.WebApp.Tests.Acceptance.Brokers;
using Attrify.WebApplication.Tests.Acceptance.Brokers;

namespace Attrify.WebApp.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class InvisibleApiTests
    {
        private readonly ApiBroker apiBroker;

        public InvisibleApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;
    }
}
