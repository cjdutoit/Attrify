// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using Tynamix.ObjectFiller;

namespace Attrify.InvisibleApi.Tests.Unit.Middlewares
{
    public partial class InvisibleApiMiddlewareTests
    {
        private static string GetRandomString() =>
            new MnemonicString().GetValue();
    }
}
