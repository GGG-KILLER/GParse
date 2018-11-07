using System;
using System.Text.RegularExpressions;
using GParse.Common.IO;
using GParse.Parsing.Abstractions.Lexing;
using GParse.Parsing.Lexing.Modules;

namespace GParse.Parsing.Lexing
{
    public class LexerBuilder<TokenTypeT> : ILexerBuilder<TokenTypeT> where TokenTypeT : Enum
    {
        // Root of the tree is always null, since we don't have
        // common prefixes in the general case.
        private readonly LexerModuleTree<TokenTypeT> Modules = new LexerModuleTree<TokenTypeT> ( );

        public void AddModule ( ILexerModule<TokenTypeT> module ) => this.Modules.AddChild ( module );

        #region AddLiteral

        public void AddLiteral ( String ID, TokenTypeT type, String raw ) =>
            this.AddModule ( new LiteralLexerModule<TokenTypeT> ( ID, type, raw ) );

        public void AddLiteral ( String ID, TokenTypeT type, String raw, Boolean isTrivia ) =>
            this.AddModule ( new LiteralLexerModule<TokenTypeT> ( ID, type, raw, isTrivia ) );

        public void AddLiteral ( String ID, TokenTypeT type, String raw, Object value ) =>
            this.AddModule ( new LiteralLexerModule<TokenTypeT> ( ID, type, raw, value ) );

        public void AddLiteral ( String ID, TokenTypeT type, String raw, Object value, Boolean isTrivia ) =>
            this.AddModule ( new LiteralLexerModule<TokenTypeT> ( ID, type, raw, value, isTrivia ) );

        #endregion AddLiteral

        #region AddRegex

        public void AddRegex ( String ID, TokenTypeT type, String regex, String prefix = null, Func<Match, Object> converter = null, Boolean isTrivia = false ) =>
            this.AddModule ( new RegexLexerModule<TokenTypeT> ( ID, type, regex, prefix, converter, isTrivia ) );

        #endregion AddRegex

        public ILexer<TokenTypeT> BuildLexer ( String input ) => this.BuildLexer ( new SourceCodeReader ( input ) );

        public ILexer<TokenTypeT> BuildLexer ( SourceCodeReader reader ) => new ModularLexer<TokenTypeT> ( this.Modules, reader );
    }
}
