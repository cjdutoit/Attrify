// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Net.Http;
using Attrify.WebApp;
using Attrify.WebApp.Tests.Acceptance;
using RESTFulSense.Clients;

namespace Attrify.WebApplication.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        internal readonly TestWebApplicationFactory<Program> webApplicationFactory;
        internal readonly HttpClient httpClient;
        internal readonly IRESTFulApiFactoryClient apiFactoryClient;

        public ApiBroker()
        {
            this.webApplicationFactory = new TestWebApplicationFactory<Program>();
            this.httpClient = this.webApplicationFactory.CreateClient();
            this.apiFactoryClient = new RESTFulApiFactoryClient(this.httpClient);
        }
    }
}