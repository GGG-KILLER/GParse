using System;
using GParse.Common.AST;

namespace GParse.Fluent.AST
{
    /// <summary>
    /// Declares a binary operation expression AST node
    /// </summary>
    public class BinaryOperatorExpression : ASTNode
    {
        /// <summary>
        /// The expression on the left side of the operator
        /// </summary>
        public readonly ASTNode LeftHandSide;

        /// <summary>
        /// The expression on the right side of the operator
        /// </summary>
        public readonly ASTNode RightHandSide;

        /// <summary>
        /// The operator itself
        /// </summary>
        public readonly String Operator;

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// <param name="op"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public BinaryOperatorExpression ( String op, ASTNode lhs, ASTNode rhs )
        {
            this.Operator = op;
            this.LeftHandSide = lhs;
            this.RightHandSide = rhs;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) => $"BinOp<{this.LeftHandSide}, {this.Operator}, {this.RightHandSide}>";
    }
}
