using System;
using GParse.Common;

namespace GParse.CLI.AST
{
    public class UnaryOperatorExpression : ASTNode
    {
        public readonly String Operator;
        public readonly ASTNode Operand;

        public UnaryOperatorExpression ( String Operator, ASTNode Operand )
        {
            this.Operator = Operator;
            this.Operand = Operand;

        }

        public override String ToString ( ) => $"Unary<{this.Operator}, {this.Operand}>";
    }
}
