// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Attrify.Attributes;
using Attrify.DeprecatedApi.Brokers.DateTimes;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Attrify.DeprecatedApi.Services.DeprecatedApiScanners
{
    public partial class DeprecatedApiScannerService : IDeprecatedApiScannerService
    {
        private readonly ApplicationPartManager partManager;
        private readonly IDateTimeBroker dateTimeBroker;

        public DeprecatedApiScannerService(ApplicationPartManager partManager, IDateTimeBroker dateTimeBroker)
        {
            this.partManager = partManager;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<List<DeprecatedApiInfo>> GetDeprecatedApisAsync() =>
        TryCatch(async () =>
        {
            var deprecatedApis = new List<DeprecatedApiInfo>();
            var controllerFeature = new ControllerFeature();
            partManager.PopulateFeature(controllerFeature);

            foreach (var controller in controllerFeature.Controllers)
            {
                var controllerName = controller.Name.Replace("Controller", "");
                var controllerRoute = controller.GetCustomAttributes<RouteAttribute>()
                    .FirstOrDefault()?.Template;

                if (controllerRoute == null)
                {
                    controllerRoute = "defaultRoute";
                }

                var actions = controller
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .Where(m => m.GetCustomAttributes<HttpMethodAttribute>().Any());

                foreach (var action in actions)
                {
                    var deprecatedAttribute = action.GetCustomAttribute<DeprecatedApiAttribute>();

                    if (deprecatedAttribute != null)
                    {
                        var httpMethods = action.GetCustomAttributes<HttpMethodAttribute>()
                            .Select(a => a.HttpMethods.FirstOrDefault())
                            .ToList();

                        RouteAttribute actionRoute = action.GetCustomAttributes<RouteAttribute>()
                            .FirstOrDefault();

                        var methodAttribute = action.GetCustomAttributes<HttpMethodAttribute>()
                            .FirstOrDefault();

                        DateTimeOffset sunsetDateTimeOffset = DateTimeOffset.Parse(deprecatedAttribute.Sunset);
                        DateTimeOffset currentDateTimeOffset = await dateTimeBroker.GetCurrentDateTimeOffsetAsync();

                        // Ensure that both controllerRoute and actionRoute are handled correctly
                        var route = controllerRoute != null && actionRoute != null
                            ? $"{controllerRoute}/{actionRoute.Template}"
                            : $"{controllerRoute}/{methodAttribute.Template}";

                        deprecatedApis.Add(new DeprecatedApiInfo
                        {
                            Controller = controllerName,
                            Action = action.Name,
                            HttpMethods = httpMethods,
                            Route = route,
                            Sunset = deprecatedAttribute.Sunset,
                            Warning = deprecatedAttribute.Warning,
                            Link = deprecatedAttribute.Link,
                            IsDeprecated = sunsetDateTimeOffset <= currentDateTimeOffset
                        });
                    }
                }
            }

            return deprecatedApis;
        });
    }
}
