// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;

namespace Attrify.Attributes
{
    /// <summary>
    /// Specifies that an API endpoint is deprecated and provides metadata about its deprecation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DeprecatedApiAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the sunset date for the API.
        /// This is the date after which the API is considered deprecated or removed.
        /// </summary>
        /// <example>2025-01-01</example>
        public string Sunset { get; set; }

        /// <summary>
        /// Gets or sets a warning message to be included in responses.
        /// The warning message should describe the deprecation and suggest alternatives if available.
        /// </summary>
        /// <example>
        /// "This API is deprecated and will be removed on 2025-01-01. Use v2 instead."
        /// </example>
        public string Warning { get; set; }

        /// <summary>
        /// Gets or sets a URL linking to additional information about the deprecation.
        /// The link should provide details about the deprecation timeline, alternatives, or migration steps.
        /// </summary>
        /// <example>https://api.example.com/deprecation-info</example>
        public string Link { get; set; }
    }
}
