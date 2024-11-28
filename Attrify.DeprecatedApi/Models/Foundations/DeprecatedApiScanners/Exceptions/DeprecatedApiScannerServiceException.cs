// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using Xeptions;

namespace Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners.Exceptions
{
    public class DeprecatedApiScannerServiceException : Xeption
    {
        public DeprecatedApiScannerServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
