﻿using System;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Tests.Parser
{
    public class MatcherTestParser : FluentParser
    {
        private readonly BaseMatcher Body;
        private readonly NodeFactory Callback;
        private readonly Boolean SetupDone;

        public MatcherTestParser ( BaseMatcher body, NodeFactory callback )
        {
            this.Body = body ?? throw new System.ArgumentNullException ( nameof ( body ) );
            this.Callback = callback ?? throw new System.ArgumentNullException ( nameof ( callback ) );
            this.SetupDone = true;
            this.Setup ( );

        }

        protected override void Setup ( )
        {
            this.RootRule ( "root", new EOFMatcher ( ) );
            if ( !this.SetupDone )
                return;
            this.RootRule ( "root", this.Body );
            this.Factory ( "root", this.Callback );
        }
    }
}