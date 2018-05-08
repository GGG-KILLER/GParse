using System;
using System.Linq.Expressions;
using System.Reflection;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class StringMatcher : BaseMatcher
    {
        private readonly String Filter;

        public StringMatcher ( String filter )
        {
            if ( String.IsNullOrEmpty ( filter ) )
                throw new ArgumentException ( "message", nameof ( filter ) );
            this.Filter = filter;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 )
        {
            return reader.IsNext ( this.Filter, offset );
        }

        private static readonly MethodInfo ReaderIsNextStringInt32 = typeof ( SourceCodeReader )
            .GetMethod ( "IsNext", new[] { typeof ( String ), typeof ( Int32 ) } );

        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            return Expression.Call ( reader, ReaderIsNextStringInt32, Expression.Constant ( this.Filter ), offset );
        }

        public override String Match ( SourceCodeReader reader )
        {
            if ( !this.IsMatch ( reader ) )
                return null;
            reader.Advance ( this.Filter.Length );
            return this.Filter;
        }

        private static readonly MethodInfo ReaderAdvanceInt32 = typeof ( SourceCodeReader )
            .GetMethod ( "Advance", new[] { typeof ( Int32 ) } );

        internal override Expression InternalMatchExpression ( ParameterExpression reader )
        {
            // reader.Advance ( this.Filter.Length )
            return Expression.Block (
                Expression.Call ( reader, ReaderAdvanceInt32, Expression.Constant ( this.Filter.Length ) ),
                Expression.Constant ( this.Filter )
            );
        }
    }
}
