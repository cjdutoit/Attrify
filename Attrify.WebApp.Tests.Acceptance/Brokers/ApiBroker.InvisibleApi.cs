// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Threading.Tasks;

namespace Attrify.WebApplication.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string invisibleApiRelativeUrl = "api/InvisibleApi";

        public async ValueTask<string> GetVisibleAsync() =>
            await this.apiFactoryClient.GetContentStringAsync($"{invisibleApiRelativeUrl}/visible");

        public async ValueTask<string> GetInvisibleAsync() =>
            await this.apiFactoryClient.GetContentStringAsync($"{invisibleApiRelativeUrl}/invisible");
    }
}
