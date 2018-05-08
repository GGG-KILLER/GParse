using System;
using System.Linq.Expressions;
using System.Reflection;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class MultiCharMatcher : BaseMatcher
    {
        internal readonly Char[] Whitelist;

        public MultiCharMatcher ( params Char[] whitelist )
        {
            this.Whitelist = whitelist;
            Array.Sort ( this.Whitelist );
        }

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 )
        {
            return !reader.EOF ( ) && Array.BinarySearch ( this.Whitelist, ( Char ) reader.Peek ( offset ) ) != -1;
        }

        public override String Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader ) ? reader.ReadString ( 1 ) : null;
        }

        private static readonly MethodInfo ReaderPeekInt32 = typeof ( SourceCodeReader )
            .GetMethod ( "Peek", new[] { typeof ( Int32 ) } );
        private static readonly MethodInfo ArrayBinarySearch = typeof ( Array )
            .GetMethod ( "BinarySearch", new[] { typeof ( Char[] ), typeof ( Char ) } );
        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            return Expression.NotEqual (
                Expression.Call ( ArrayBinarySearch, Expression.Constant ( this.Whitelist ), Expression.Call ( reader, ReaderPeekInt32, offset ) ),
                Expression.Constant ( -1 )
            );
        }

        private static readonly MethodInfo ReaderReadStringInt32 = typeof ( SourceCodeReader )
            .GetMethod ( "ReadString", new[] { typeof ( Int32 ) } );
        internal override Expression InternalMatchExpression ( ParameterExpression reader, ParameterExpression MatchedListener )
        {
            return Expression.Call ( reader, ReaderReadStringInt32, Expression.Constant ( 1 ) );
        }
    }
}
