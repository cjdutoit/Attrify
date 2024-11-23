// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;

namespace Attrify.Attributes
{
    /// <summary>
    /// Marks an API endpoint or controller as invisible by default.
    /// </summary>
    /// <remarks>
    /// When an endpoint or controller is decorated with this attribute, 
    /// it becomes inaccessible unless the following conditions are met:
    /// <list type="number">
    /// <item>
    /// <description>
    /// The request includes a specific header with the correct key-value pair.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// The user is authenticated and belongs to a role matching the specified key in the header.
    /// </description>
    /// </item>
    /// </list>
    /// If either of the above conditions is not satisfied, the API will return a 404 Not Found status.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class InvisibleApiAttribute : Attribute
    { }
}
