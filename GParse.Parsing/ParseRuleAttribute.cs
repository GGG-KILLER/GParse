using System;

namespace GParse.Parsing
{
    public sealed class ParseRuleAttribute : Attribute
    {
        public readonly String Name;

        public ParseRuleAttribute ( String Name )
        {
            this.Name = Name;
        }
    }
}
