// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Collections.Generic;

namespace Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners
{
    /// <summary>
    /// Represents metadata about a deprecated API, including its controller, action, 
    /// HTTP methods, route, and deprecation details.
    /// </summary>
    public class DeprecatedApiInfo
    {
        /// <summary>
        /// Gets or sets the name of the controller containing the deprecated API.
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets the name of the action method marked as deprecated.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the list of HTTP methods (e.g., GET, POST) supported by the deprecated API.
        /// </summary>
        public List<string> HttpMethods { get; set; }

        /// <summary>
        /// Gets or sets the route template for the deprecated API endpoint.
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Gets or sets the sunset date, indicating when the API is or was deprecated.
        /// Expected format: ISO 8601 (e.g., "2024-11-28").
        /// </summary>
        public string Sunset { get; set; }

        /// <summary>
        /// Gets or sets a warning message providing additional details about the deprecation, 
        /// such as alternative APIs or migration instructions.
        /// </summary>
        public string Warning { get; set; }

        /// <summary>
        /// Gets or sets a link to further information about the API deprecation, 
        /// such as documentation or migration guides.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the API is fully deprecated. 
        /// True if the sunset date has passed; false if it is pending deprecation.
        /// </summary>
        public bool IsDeprecated { get; set; }
    }

}
