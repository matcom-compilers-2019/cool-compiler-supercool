﻿using SuperCOOL.Constants;

namespace SuperCOOL.CodeGeneration.CIL.AST
{
    public class ASTCILLessThanTwoConstantNode : ASTCILExpressionNode
    {
        public int Left { get; }
        public int Right { get; }

        public ASTCILLessThanTwoConstantNode( int left, int right ) : base( Types.Bool )
        {
            Left = left;
            Right = right;
        }

        public override Result Accept<Result>( ICILVisitor<Result> Visitor ) => Visitor.VisitLessThanTwoConstant( this );
    }
}