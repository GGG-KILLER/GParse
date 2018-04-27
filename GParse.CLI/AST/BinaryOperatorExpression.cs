﻿using System;
using GParse.Common;

namespace GParse.CLI.AST
{
    public class BinaryOperatorExpression : ASTNode
    {
        public readonly ASTNode LeftHandSide;
        public readonly ASTNode RightHandSide;
        public readonly String Operator;

        public BinaryOperatorExpression ( String Operator, ASTNode lhs, ASTNode rhs )
        {
            this.Operator = Operator;
            this.LeftHandSide = lhs;
            this.RightHandSide = rhs;
        }

        public override String ToString ( ) => $"BinOp<{this.LeftHandSide}, {this.Operator}, {this.RightHandSide}>";
    }
}