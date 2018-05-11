using System;
using System.Linq.Expressions;
using System.Reflection;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class CharMatcher : BaseMatcher
    {
        internal readonly Char Filter;

        public CharMatcher ( Char filter )
        {
            this.Filter = filter;
        }

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 )
        {
            return this.Filter == reader.Peek ( offset );
        }

        public override String Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader ) ? reader.ReadString ( 1 ) : null;
        }

        private static readonly MethodInfo ReaderPeekInt32 = typeof ( SourceCodeReader )
            .GetMethod ( "Peek", new[] { typeof ( Int32 ) } );
        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            return Expression.Equal (
                Expression.Constant ( this.Filter ),
                Expression.Convert ( Expression.Call ( reader, ReaderPeekInt32, offset ) , typeof ( Char ) ) );
        }

        private static readonly MethodInfo ReaderReadStringInt32 = typeof ( SourceCodeReader )
            .GetMethod ( "ReadString", new[] { typeof ( Int32 ) } );
        internal override Expression InternalMatchExpression ( ParameterExpression reader )
        {
            return Expression.Call ( reader, ReaderReadStringInt32, Expression.Constant ( 1 ) );
        }
    }
}
