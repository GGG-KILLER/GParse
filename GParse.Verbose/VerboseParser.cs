using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common;
using GParse.Verbose.Abstractions;
using GParse.Verbose.Matchers;
using GParse.Verbose.Results;

namespace GParse.Verbose
{
    public abstract class VerboseParser
    {
        protected VerboseParser ( )
        {
        }

        protected static BaseMatcher MatchParser ( String pattern )
        {
            BaseMatcher matcher = null;
            var lastIdx = pattern.Length - 1;

            if ( pattern[0] == '[' && pattern[lastIdx] == ']' )
            {
                // Range
                for ( var i = 1; i < lastIdx; i++ )
                {
                    BaseMatcher match;
                    var start = pattern[i];
                    if ( pattern[i + 1] == '-' )
                    {
                        var end = pattern[i + 2];
                        i += 2;

                        match = Match.CharRange ( start, end );
                    }
                    else
                        match = Match.Char ( start );
                    matcher = matcher?.Or ( match ) ?? match;
                }
            }
            else
            {
                matcher = pattern.Length == 1
                    ? Match.Char ( pattern[0] )
                    : Match.String ( pattern );
            }

            return matcher;
        }

        public abstract BaseMatcher GenerateRules ( );
        public abstract void GenerateParsers ( Action<String, Func<MatchResult[], ASTNode>> RegisterRuleParser );
    }
}
