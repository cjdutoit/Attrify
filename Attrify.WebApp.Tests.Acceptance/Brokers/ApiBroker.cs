// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Net.Http;
using Attrify.InvisibleApi.Models;
using Attrify.WebApp;
using Attrify.WebApp.Tests.Acceptance;
using Microsoft.Extensions.DependencyInjection;
using RESTFulSense.Clients;

namespace Attrify.WebApplication.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        internal readonly TestWebApplicationFactory<Program> webApplicationFactory;
        internal readonly HttpClient httpClient;
        internal readonly IRESTFulApiFactoryClient apiFactoryClient;
        internal readonly InvisibleApiKey invisibleApiKey;

        public ApiBroker()
        {
            this.webApplicationFactory = new TestWebApplicationFactory<Program>();
            this.httpClient = this.webApplicationFactory.CreateClient();
            this.apiFactoryClient = new RESTFulApiFactoryClient(this.httpClient);
            this.invisibleApiKey = this.webApplicationFactory.Services.GetService<InvisibleApiKey>();
        }
    }
}