﻿using SuperCOOL.Constants;

namespace SuperCOOL.CodeGeneration.CIL.AST
{
    public class ASTCILAddTwoVariablesNode : ASTCILExpressionNode
    {
        public ASTCILExpressionNode Left { get; }
        public ASTCILExpressionNode Right { get; }

        public ASTCILAddTwoVariablesNode(ASTCILExpressionNode left, ASTCILExpressionNode right) :base(Types.Int)
        {
            Left = left;
            Right = right;
        }

        public override Result Accept<Result>( ICILVisitor<Result> Visitor ) => Visitor.VisitAddTwoVariables( this );
    }
}