// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Threading.Tasks;

namespace Attrify.WebApplication.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string deprecatedApiRelativeUrl = "api/DeprecatedApi";

        public async ValueTask<string> GetActiveAsync() =>
            await this.apiFactoryClient.GetContentStringAsync($"{deprecatedApiRelativeUrl}/active");

        public async ValueTask<dynamic> GetPendingAsync() =>
            await this.apiFactoryClient.GetContentAsync<dynamic>($"{deprecatedApiRelativeUrl}/pending");

        public async ValueTask<dynamic> GetdeprecatedAsync() =>
            await this.apiFactoryClient.GetContentAsync<dynamic>($"{deprecatedApiRelativeUrl}/pending");
    }
}
