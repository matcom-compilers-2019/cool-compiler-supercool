﻿using SuperCOOL.Constants;

namespace SuperCOOL.CodeGeneration.CIL.AST
{
    public class ASTCILMinusTwoConstantNode : ASTCILExpressionNode
    {
        public int Left { get; }
        public int Right { get; }

        public ASTCILMinusTwoConstantNode( int left, int right ) : base( Types.Int )
        {
            Left = left;
            Right = right;
        }

        public override Result Accept<Result>( ICILVisitor<Result> Visitor ) => Visitor.VisitMinusTwoConstant( this );
    }
}