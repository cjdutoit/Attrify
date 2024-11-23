// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using Tynamix.ObjectFiller;

namespace Attrify.DeprecatedApi.Tests.Unit.Middlewares
{
    public partial class DeprecatedApiMiddlewareTests
    {
        private static string GetRandomString() =>
            new MnemonicString().GetValue();
    }
}
