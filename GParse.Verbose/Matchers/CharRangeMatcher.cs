using System;
using System.Linq.Expressions;
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
            this.Start = ( Char ) Math.Max ( Start, End );
            this.End = ( Char ) Math.Min ( Start, End );
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

        internal override Expression InternalIsMatchExpression ( ParameterExpression reader, Expression offset )
        {
            ParameterExpression ch = Expression.Variable ( typeof ( Char ) );
            LabelTarget @return = Expression.Label ( typeof ( Boolean ) );
            return Expression.Block (
                Expression.Assign ( ch, Expression.Call ( reader, ReaderPeek, offset ) ),
                Expression.Add (
                    Expression.LessThan ( Expression.Constant ( this.Start ), ch ),
                    Expression.GreaterThan ( Expression.Constant ( this.End ), ch )
                ),
                Expression.Label ( @return )
            );
        }

        public override String Match ( SourceCodeReader reader )
        {
            return this.IsMatch ( reader ) ? reader.ReadString ( 1 ) : null;
        }

        internal override Expression InternalMatchExpression ( ParameterExpression reader )
        {
            return Expression.Call ( reader, ReaderReadString, Expression.Constant ( 1 ) );
        }
    }
}
