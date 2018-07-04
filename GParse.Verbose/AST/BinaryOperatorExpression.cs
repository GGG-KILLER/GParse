using System;
using GParse.Common.AST;

namespace GParse.Verbose.AST

{
    public class BinaryOperatorExpression : ASTNode
    {
        public readonly ASTNode LeftHandSide;
        public readonly ASTNode RightHandSide;
        public readonly String Operator;

        public BinaryOperatorExpression ( String op, ASTNode lhs, ASTNode rhs )
        {
            this.Operator = op;
            this.LeftHandSide = lhs;
            this.RightHandSide = rhs;
        }

        public override String ToString ( ) => $"BinOp<{this.LeftHandSide}, {this.Operator}, {this.RightHandSide}>";
    }
}
