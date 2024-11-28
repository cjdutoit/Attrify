// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;
using Xeptions;

namespace Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners.Exceptions
{
    public class FailedServiceDeprecatedApiScannerExceptions : Xeption
    {
        public FailedServiceDeprecatedApiScannerExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
