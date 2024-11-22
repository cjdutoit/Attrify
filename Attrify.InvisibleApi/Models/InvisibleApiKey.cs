// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;

namespace Attrify.InvisibleApi.Models
{
    public class InvisibleApiKey
    {
        public InvisibleApiKey()
        {
            Key = $"InvisibleApi-{Guid.NewGuid()}";
            Value = $"{Guid.NewGuid()}-{Guid.NewGuid()}";
        }

        public InvisibleApiKey(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }
    }
}
