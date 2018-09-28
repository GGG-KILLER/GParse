using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using GParse.Common;
using GParse.Common.IO;
using GParse.Common.Lexing;
using GParse.Common.Utilities;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Exceptions;
using GParse.Fluent.Matchers;
using GParse.Fluent.Visitors;

namespace GParse.Fluent.Lexing.Compiler
{
    public class LexTreeCompiler : IMatcherTreeVisitor<Expression>
    {
        // Expression state
        private ParameterExpression BufferStack;
        private ParameterExpression Reader;
        private ParameterExpression Lexer;
        private LabelTarget ReturnLabel;

        // Compiler state
        private readonly FluentLexer LexerInst;
        private String Name;
        private Func<String, String, Object, TokenType, SourceRange, Token> TokenFactory;
        private Stack<String> RuleStack;
        private Int32 LocalVarCount;
        private Stack<FailureHandleInfo> FailureHandlingStack;
        private static readonly ExpressionReconstructor reconstructor = new ExpressionReconstructor ( );

        public LexTreeCompiler ( FluentLexer lexer )
        {
            this.LexerInst = lexer;
        }

        #region Helper functions

        #region Buffer Stack Management

        /// <summary>
        /// Pushes a buffer on top of the buffer stack (at runtime)
        /// </summary>
        /// <returns></returns>
        private MethodCallExpression PushBuffer ( )
        {
            return ExprUtils.MethodCall<Stack<StringBuilder>, StringBuilder> (
                this.BufferStack, "Push", Expression.New ( typeof ( StringBuilder ) ) );
        }

        /// <summary>
        /// Gets the buffer on the top of the buffer stack (at runtime)
        /// </summary>
        /// <returns></returns>
        private MethodCallExpression GetTopBuffer ( )
        {
            return ExprUtils.MethodCall<Stack<StringBuilder>> ( this.BufferStack, "Peek" );
        }

        /// <summary>
        /// Pops a buffer from the top of the buffer stack (at runtime)
        /// </summary>
        /// <returns></returns>
        private MethodCallExpression PopTopBuffer ( )
        {
            return ExprUtils.MethodCall<Stack<StringBuilder>> (
                this.BufferStack, "Pop" );
        }

        #endregion Buffer Stack Management

        #region Locals and Labels Management

        /// <summary>
        /// Returns a local variable with the provided prefix
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private ParameterExpression GetLocal<T> ( String prefix )
            => Expression.Parameter ( typeof ( T ), $"{prefix}_{this.LocalVarCount++:N3}" );

        /// <summary>
        /// Returns a label with the provided prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private LabelTarget GetLabel ( String prefix )
            => Expression.Label ( $"{prefix}_{this.LocalVarCount++:N3}" );

        /// <summary>
        /// Returns a label with the provided prefix and the
        /// specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private LabelTarget GetLabel<T> ( String prefix )
            => Expression.Label ( typeof ( T ), $"{prefix}_{this.LocalVarCount++:N3}" );

        #endregion Locals and Labels Management

        #region Fail-Safe Loop Management

        private (LabelTarget Primary, LabelTarget Secondary) GetFailSafeLabels ( )
        {
            return (this.GetLabel ( "primary" ), this.GetLabel ( "secondary" ));
        }

        /// <summary>
        /// Returns a fail-safe loop (handles the
        /// <see cref="FailureHandlingStack" /> itself)
        /// </summary>
        /// <param name="body">
        /// Delegate that generates the loop body (first and
        /// second arguments are break and continue respectively)
        /// </param>
        /// <param name="preferContinue">
        /// Whether continue should be picked on failure other
        /// than break
        /// </param>
        /// <returns></returns>
        private LoopExpression GetFailSafeLoop ( Func<LabelTarget, LabelTarget, Expression> body, Boolean preferContinue = false )
        {
            try
            {
                (LabelTarget Break, LabelTarget Continue) = this.GetFailSafeLabels ( );
                this.FailureHandlingStack.Push ( new FailureHandleInfo ( FailSafeLocation.Loop,
                    preferContinue ? LabelTargetPreference.Secondary : LabelTargetPreference.Primary,
                    Break,
                    Continue ) );
                return Expression.Loop ( body?.Invoke ( Break, Continue ), Break, Continue );
            }
            finally
            {
                this.FailureHandlingStack.Pop ( );
            }
        }

        /// <summary>
        /// Gets a fail-safe block
        /// </summary>
        /// <param name="locals">
        /// Locals that should exist in the block
        /// </param>
        /// <param name="body">           Body of the block</param>
        /// <param name="preferSecondary">
        /// Whether the secondary label should be prefered to the
        /// primary for failure jumps
        /// </param>
        /// <returns></returns>
        private BlockExpression GetFailSafeBlock ( IEnumerable<ParameterExpression> locals, Func<LabelTarget, LabelTarget, IEnumerable<Expression>> body, Boolean preferSecondary = false )
        {
            try
            {
                (LabelTarget Primary, LabelTarget Secondary) = this.GetFailSafeLabels ( );
                this.FailureHandlingStack.Push ( new FailureHandleInfo ( FailSafeLocation.Other,
                    preferSecondary ? LabelTargetPreference.Secondary : LabelTargetPreference.Primary, Primary, Secondary ) );
                return Expression.Block ( locals, body?.Invoke ( Primary, Secondary ) );
            }
            finally
            {
                this.FailureHandlingStack.Pop ( );
            }
        }

        /// <summary>
        /// Fails the execution of the current rule. Returns the
        /// appropriate expression based on whether we're
        /// currently inside an fail-safe environment or not.
        /// </summary>
        /// <param name="message">
        /// The message to be given to the exception in the case
        /// we're outside of a fail-safe loop
        /// </param>
        /// <returns></returns>
        private Expression Fail ( String message )
        {
            if ( this.FailureHandlingStack.Count > 0 )
            {
                FailureHandleInfo info = this.FailureHandlingStack.Peek ( );
                switch ( info.Location )
                {
                    case FailSafeLocation.Loop:
                        return info.Preference == LabelTargetPreference.Primary
                            ? Expression.Break ( info.Primary )
                            : Expression.Continue ( info.Secondary );

                    case FailSafeLocation.Other:
                        return Expression.Goto ( info.Preference == LabelTargetPreference.Primary ? info.Primary : info.Secondary );

                    default:
                        throw new InvalidOperationException ( "Invalid FailSafeLocation." );
                }
            }
            return ExprUtils.Throw<MatcherFailureException, SourceLocation, String> (
                    ExprUtils.PropertyGet<SourceCodeReader> ( this.Reader, "Location" ),
                    Expression.Constant ( message ) );
        }

        #endregion Fail-Safe Loop Management

        private static String Reconstruct ( BaseMatcher matcher )
        {
            lock ( reconstructor )
                return matcher.Accept ( reconstructor );
        }

        #endregion Helper functions

        public Func<SourceCodeReader, FluentLexer, Token> Compile ( RuleDefinition definition, Func<String, String, Object, TokenType, SourceRange, Token> tokenFactory )
        {
            Func<String, Object> conv = definition.Converter;
            TokenType type = definition.Type;
            BaseMatcher tree = definition.Body;

            // Reset the compiler state
            this.Name = definition.Name;
            this.RuleStack = new Stack<String> ( );
            this.TokenFactory = tokenFactory;
            this.LocalVarCount = 0;
            this.FailureHandlingStack = new Stack<FailureHandleInfo> ( );

            // Initialize the function state
            this.Reader = Expression.Parameter ( typeof ( SourceCodeReader ), "reader" );
            this.Lexer = Expression.Parameter ( typeof ( FluentLexer ), "lexer" );
            this.BufferStack = this.GetLocal<Stack<StringBuilder>> ( "bufferStack" );
            this.ReturnLabel = Expression.Label ( typeof ( Token ), "return" );

            // Get and save initial location
            ParameterExpression start = this.GetLocal<SourceLocation> ( "start" );
            ParameterExpression finalStr = this.GetLocal<String> ( "finalStr" );
            ParameterExpression convertedValue = this.GetLocal<Object> ( "converted" );
            return Expression.Lambda<Func<SourceCodeReader, FluentLexer, Token>> (
                Expression.Block (
                    typeof ( Token ),
                    new[] { start, convertedValue, this.BufferStack, finalStr, },
                    /* bufferStack = new Stack<StringBuilder> ( ) */
                    Expression.Assign ( this.BufferStack, Expression.New ( typeof ( Stack<StringBuilder> ) ) ),
                    /* bufferStack.Push ( new StringBuilder ( ) ) */
                    this.PushBuffer ( ),
                    /* start = reader.Location */
                    Expression.Assign ( start, ExprUtils.PropertyGet<SourceCodeReader> ( this.Reader, "Location" ) ),
                    /* ... */
                    tree.Accept ( this ),
                    /* finalStr = this.BufferStack.Pop ( ).ToString ( ) */
                    Expression.Assign ( finalStr, ExprUtils.MethodCall<StringBuilder> ( this.PopTopBuffer ( ), "ToString" ) ),
                    /* conv != null : converted = conv ( finalStr ) else converted = finalStr */
                    Expression.Assign ( convertedValue, conv != null ? ( Expression ) ExprUtils.Call ( conv, finalStr ) : finalStr ),
                    /* return tokenFactory ( name, finalStr, converted, type, start.To ( reader.Location ) ) */
                    Expression.Return ( this.ReturnLabel,
                        ExprUtils.Call ( tokenFactory,
                            Expression.Constant ( this.Name ),
                            finalStr,
                            convertedValue,
                            Expression.Constant ( type ),
                            ExprUtils.MethodCall<SourceLocation, SourceLocation> ( start, "To",
                                ExprUtils.PropertyGet<SourceCodeReader> ( this.Reader, "Location" ) )
                        )
                    ),
                    Expression.Label ( this.ReturnLabel, Expression.Constant ( null ) )
                ),
                $"GetRule_{this.Name}",
                new[] { this.Reader, this.Lexer }
            ).Compile ( );
        }

        public Expression Visit ( SequentialMatcher SequentialMatcher )
        {
            throw new NotImplementedException ( );
        }

        public Expression Visit ( AlternatedMatcher AlternatedMatcher )
        {
            throw new NotImplementedException ( );
        }

        public Expression Visit ( CharMatcher charMatcher )
        {
            throw new NotImplementedException ( );
        }

        public Expression Visit ( RangeMatcher RangeMatcher )
        {
            throw new NotImplementedException ( );
        }

        public Expression Visit ( EOFMatcher eofMatcher )
        {
            throw new NotImplementedException ( );
        }

        public Expression Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            throw new NotImplementedException ( );
        }

        public Expression Visit ( IgnoreMatcher ignoreMatcher )
        {
            throw new NotImplementedException ( );
        }

        public Expression Visit ( JoinMatcher joinMatcher )
        {
            throw new NotImplementedException ( );
        }

        public Expression Visit ( MarkerMatcher markerMatcher )
        {
            throw new NotImplementedException ( );
        }

        public Expression Visit ( CharListMatcher CharListMatcher )
        {
            Expression peek = ExprUtils.MethodCall<SourceCodeReader> ( this.Reader, "Peek" );
            Expression condition = Expression.Equal ( peek, Expression.Constant ( CharListMatcher.Whitelist[0] ) );
            for ( var i = 1; i < CharListMatcher.Whitelist.Length; i++ )
                condition = Expression.Or ( condition, Expression.Equal ( peek, Expression.Constant ( CharListMatcher.Whitelist[i] ) ) );
            /* if ( condition )*/
            return Expression.IfThenElse ( condition,
                /* bufferStack.Peek ( ).Append ( ( Char ) reader.ReadChar ( ) ); */
                ExprUtils.MethodCall<StringBuilder, Char> ( this.GetTopBuffer ( ), "Append",
                    Expression.Convert ( ExprUtils.MethodCall<SourceCodeReader> ( this.Reader, "ReadChar" ), typeof ( Char ) ) ),
                this.Fail ( $"Expected character inside whitelist ['{String.Join ( "', '", Array.ConvertAll ( CharListMatcher.Whitelist, StringUtilities.GetCharacterRepresentation ) )}']" ) );
        }

        public Expression Visit ( NegatedMatcher negatedMatcher )
        {
            ParameterExpression start = this.GetLocal<SourceLocation> ( "start" );
            return this.GetFailSafeBlock ( new[] { start }, ( primary, secondary ) => new[]
            {
                /* start = reader.Location; */
                Expression.Assign ( start, ExprUtils.PropertyGet<SourceCodeReader> ( this.Reader, "Location" ) ),
                /* bufferStack.Push ( new StringBuilder ( ) ); */
                this.PushBuffer ( ),
                /* ... */
                negatedMatcher.PatternMatcher.Accept ( this ),
            /* primary:; // Used in case of success (which we don't want) */
            Expression.Label ( primary ),
                /* bufferStack.Pop ( ); */
                this.PopTopBuffer ( ),
                /* reader.Rewind ( start ); */
                ExprUtils.MethodCall<SourceCodeReader, SourceLocation> ( this.Reader, "Rewind", start ),
                /* fail ( "Matched expression when it wasn't meant to be matched: expr" ); */
                this.Fail ( $"Matched expression when it wasn't meant to be matched: {Reconstruct ( negatedMatcher )}" ),
            /* secondary:; // Used in case of failure (which we want) */
            Expression.Label ( secondary ),
                /* bufferStack.Pop ( ); */
                this.PopTopBuffer ( ),
                /* reader.Rewind ( start ); */
                ExprUtils.MethodCall<SourceCodeReader, SourceLocation> ( this.Reader, "Rewind", start ),
                /* bufferStack.Peek ( ).Append ( ( Char ) reader.ReadChar ( ) ); */
                ExprUtils.MethodCall<StringBuilder, Char> ( this.GetTopBuffer ( ), "Append",
                    Expression.Convert ( ExprUtils.MethodCall<SourceCodeReader> ( this.Reader, "ReadChar" ), typeof ( Char ) ) )
            }, true );
        }

        public Expression Visit ( OptionalMatcher optionalMatcher )
        {
            ParameterExpression start = this.GetLocal<SourceLocation> ( "start" ),
                str = this.GetLocal<String> ( "str" );
            /* { */
            return this.GetFailSafeBlock ( new[] { start, str, },
                ( primary, secondary ) => new Expression[]
                {
                    /* start = reader.Location; */
                    Expression.Assign ( start, ExprUtils.PropertyGet<SourceCodeReader> ( this.Reader, "Location" ) ),
                    /* bufferStack.Push ( new StringBuilder ( ) ); */
                    this.PushBuffer ( ),
                    /* ... */
                    optionalMatcher.PatternMatcher.Accept ( this ),
                    /* str = bufferStack.Pop ( ).ToString ( ); */
                    Expression.Assign ( str, ExprUtils.MethodCall<StringBuilder> ( this.PopTopBuffer ( ), "ToString" ) ),
                    /* bufferStack.Peek ( ).Append ( str ); */
                    ExprUtils.MethodCall<StringBuilder, String> ( this.GetTopBuffer ( ), "Append", str ),
                    /* goto secondary; */
                    Expression.Goto ( secondary ),
                    /* primary:; // Used in case of failure (fail-safe block is configured to prefer primary label) */
                    Expression.Label ( primary ),
                        /* bufferStack.Pop ( ) */
                        this.PopTopBuffer ( ),
                        /* reader.Rewind ( start ) */
                        ExprUtils.MethodCall<SourceCodeReader, SourceLocation> ( this.Reader, "Rewind", start ),
                    /* secondary:; */
                    Expression.Label ( secondary )
                } );
            /* } */
        }

        public Expression Visit ( RepeatedMatcher repeatedMatcher )
        {
            ParameterExpression i = this.GetLocal<Int32> ( "i" );
            ParameterExpression str = this.GetLocal<String> ( "str" );
            /* { */
            return Expression.Block (
                /* Int32 i;
                 * String str; */
                new[] { i, str },
                /* i = 0; */
                Expression.Assign ( i, Expression.Constant ( 0 ) ),
                /* bufferStack.Push ( new StringBuilder ( ) ); */
                this.PushBuffer ( ),
                /* while ( true )
                 * { */
                this.GetFailSafeLoop ( ( @break, _ ) => Expression.IfThenElse (
                    /* if ( i <= end ) */
                    Expression.LessThanOrEqual ( i, Expression.Constant ( repeatedMatcher.Range.End ) ),
                    /* { */
                    Expression.Block (
                        /* ... */
                        repeatedMatcher.PatternMatcher.Accept ( this ),
                        /* i++; */
                        Expression.Increment ( i )
                    ),
                    /* }
                     * else break; */
                    Expression.Break ( @break )
                ) ),
                /* } */
                Expression.IfThen (
                    /* if ( i < start ) */
                    Expression.LessThan ( i, Expression.Constant ( repeatedMatcher.Range.Start ) ),
                    /* fail ( "Failed to match expression (expr) the minimum amount of times." ); */
                    this.Fail ( $"Failed to match expression ({Reconstruct ( repeatedMatcher )}) the minimum amount of times." )
                ),
                /* str = bufferStack.Pop ( ).ToString ( ); */
                Expression.Assign ( str, ExprUtils.MethodCall<StringBuilder> ( this.PopTopBuffer ( ), "ToString" ) ),
                /* bufferStack.Peek ( ).Append ( str ); */
                ExprUtils.MethodCall<StringBuilder, String> ( this.GetTopBuffer ( ), "Append", str )
            );
            /* } */
        }

        public Expression Visit ( RulePlaceholder rulePlaceholder )
        {
            ParameterExpression tok = this.GetLocal<Token> ( "tok" );
            /* { */
            return Expression.Block (
                /* Token tok; */
                new[] { tok },
                /* tok = lexer.CompiledRules[name].Invoke ( reader, lexer ); */
                Expression.Assign (
                    tok,
                    ExprUtils.MethodCall<Func<SourceCodeReader, FluentLexer, Token>, SourceCodeReader, FluentLexer> (
                        ExprUtils.IndexGet ( ExprUtils.FieldGet<FluentLexer> ( this.Lexer, "CompiledRules" ),
                            Expression.Constant ( rulePlaceholder.Name ) ),
                        "Invoke",
                        this.Reader,
                        this.Lexer ) ),
                /* bufferStack.Peek ( ).Append ( tok.Match ); */
                ExprUtils.MethodCall<StringBuilder, String> ( this.GetTopBuffer ( ), "Append", ExprUtils.FieldGet<Token> ( tok, "Match" ) )
            );
            /* } */
        }

        public Expression Visit ( RuleWrapper ruleWrapper )
        {
            /* ... */
            return ruleWrapper.PatternMatcher.Accept ( this );
        }

        public Expression Visit ( StringMatcher stringMatcher )
        {
            ConstantExpression expected = Expression.Constant ( stringMatcher.StringFilter );
            return Expression.IfThenElse (
                /* if ( !reader.EOF ( ) && reader.IsNext ( str ) ) */
                Expression.And (
                    Expression.Not ( ExprUtils.MethodCall<SourceCodeReader> ( this.Reader, "EOF" ) ),
                    ExprUtils.MethodCall<SourceCodeReader, String> ( this.Reader, "IsNext", expected )
                ),
                /* { */
                Expression.Block (
                    /* reader.Advance ( str.Length ); */
                    ExprUtils.MethodCall<SourceCodeReader, Int32> ( this.Reader, "Advance", Expression.Constant ( stringMatcher.StringFilter.Length ) ),
                    /* bufferStack.Peek ( ).Append ( str ); */
                    ExprUtils.MethodCall<StringBuilder, String> ( this.GetTopBuffer ( ), "Append", expected )
                ),
                /* }
                 * else fail ( "Expected 'filter' but got something else." ); */
                this.Fail ( $"Expected '{stringMatcher.StringFilter}' but got something else." )
            );
        }

        public Expression Visit ( SavingMatcher savingMatcher )
        {
            Expression dict = ExprUtils.FieldGet<FluentLexer> ( this.Lexer, "SaveMemory" );
            ConstantExpression name = Expression.Constant ( savingMatcher.SaveName );
            /* { */
            return Expression.Block (
                /* bufferStack.Push ( new StringBuilder ( ) ) */
                this.PushBuffer ( ),
                /* ... */
                savingMatcher.PatternMatcher.Accept ( this ),
                /* lexer.SaveMemory[name] = bufferStack.Pop ( ).ToString ( ); */
                ExprUtils.IndexSet ( dict, name, ExprUtils.MethodCall<StringBuilder> ( this.PopTopBuffer ( ), "ToString" ) ),
                /* bufferStack.Peek ( ).Append ( lexer.SaveMemory[name] ); */
                ExprUtils.MethodCall<StringBuilder, String> ( this.GetTopBuffer ( ), "Append", ExprUtils.IndexGet ( dict, name ) )
            );
            /* } */
        }

        public Expression Visit ( LoadingMatcher loadingMatcher )
        {
            MemberExpression dict = ExprUtils.FieldGet<FluentLexer> ( this.Lexer, "SaveMemory" );
            ConstantExpression name = Expression.Constant ( loadingMatcher.SaveName );

            return Expression.IfThen (
                /* if ( lexer.SaveMemory.ContainsKey ( name ) && reader.IsNext ( lexer.SaveMemory[name] ) ) */
                Expression.And (
                    ExprUtils.MethodCall<Dictionary<String, String>, String> ( dict, "ContainsKey", name ),
                    ExprUtils.MethodCall<SourceCodeReader, String> ( this.Reader, "IsNext", ExprUtils.IndexGet ( dict, name ) )
                ),
                /* bufferStack.Peek ( ).Append ( this.Lexer.SaveMemory[name] ); */
                ExprUtils.MethodCall<StringBuilder, String> ( this.GetTopBuffer ( ), "Append", ExprUtils.IndexGet ( dict, name ) )
            );
        }
    }
}
