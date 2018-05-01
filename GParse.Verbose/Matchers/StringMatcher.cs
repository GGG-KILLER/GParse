using System;
using System.Linq.Expressions;
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

        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            return Expression.Call ( reader, ReaderIsNext, Expression.Constant ( this.Filter ), offset );
        }

        public override String Match ( SourceCodeReader reader )
        {
            if ( !this.IsMatch ( reader ) )
                return null;
            reader.Advance ( this.Filter.Length );
            return this.Filter;
        }

        internal override Expression InternalMatchExpression ( ParameterExpression reader )
        {
            // reader.Advance ( this.Filter.Length )
            return Expression.Block (
                Expression.Call ( reader, ReaderAdvance, Expression.Constant ( this.Filter.Length ) ),
                Expression.Constant ( this.Filter )
            );
        }
    }
}
