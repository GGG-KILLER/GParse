using System;
using System.Linq.Expressions;
using System.Reflection;
using GParse.Common.IO;

namespace GParse.Verbose.Matchers
{
    internal class CharRangeMatcher : BaseMatcher
    {
        internal readonly Boolean Strict;
        internal readonly Char Start;
        internal readonly Char End;

        /// <summary>
        /// </summary>
        /// <param name="Start">Interval start</param>
        /// <param name="End">Interval end</param>
        /// <param name="Strict">
        /// Whether to use Start &lt; value &lt; End instead of
        /// Start ≤ value ≤ End
        /// </param>
        public CharRangeMatcher ( Char Start, Char End, Boolean Strict )
        {
            this.Start = ( Char ) Math.Min ( Start, End );
            this.End = ( Char ) Math.Max ( Start, End );
            this.Strict = Strict;

            if ( !this.Strict )
            {
                this.Start--;
                this.End++;
            }
        }

        public override Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 )
        {
            var ch = reader.Peek ( offset );
            return this.Start < ch && ch < this.End;
        }

        private static readonly MethodInfo ReaderPeekInt32 = typeof ( SourceCodeReader ).GetMethod ( "Peek", new[] { typeof ( Int32 ) } );
        private static readonly ConstantExpression False = Expression.Constant ( false );
        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            ParameterExpression ch = Expression.Variable ( typeof ( Char ) );
            LabelTarget @return = Expression.Label ( typeof ( Boolean ), GetLabelName ( "CharRangeMatcherReturn" ) );
            return Expression.Block (
                new[] { ch },
                Expression.Assign ( ch, Expression.Convert ( Expression.Call ( reader, ReaderPeekInt32, offset ), typeof ( Char ) ) ),
                Expression.Return ( @return, Expression.And (
                    Expression.LessThan ( Expression.Constant ( this.Start ), ch ),
                    Expression.GreaterThan ( Expression.Constant ( this.End ), ch )
                ) ),
                Expression.Label ( @return, False )
            );
        }

        public override String Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader ) ? reader.ReadString ( 1 ) : null;
        }

        private static readonly MethodInfo ReaderReadStringInt32 = typeof ( SourceCodeReader )
            .GetMethod ( "ReadString", new[] { typeof ( Int32 ) } );
        internal override Expression InternalMatchExpression ( ParameterExpression reader )
        {
            return Expression.Call ( reader, ReaderReadStringInt32, Expression.Constant ( 1 ) );
        }
    }
}
