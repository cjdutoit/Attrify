// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Attrify.DeprecatedApi.Tests.Unit.Fakes
{
    public class TestApplicationPart : ApplicationPart, IApplicationPartTypeProvider
    {
        private readonly Type[] types;
        public override string Name => "TestApplicationPart";

        public TestApplicationPart(params Type[] types)
        {
            this.types = types;
        }

        public IEnumerable<TypeInfo> Types => types.Select(t => t.GetTypeInfo());
    }
}
