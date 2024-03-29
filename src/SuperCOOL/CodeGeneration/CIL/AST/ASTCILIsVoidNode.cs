﻿using SuperCOOL.Constants;

namespace SuperCOOL.CodeGeneration.CIL.AST
{
    public class ASTCILIsVoidNode : ASTCILExpressionNode
    {
        public ASTCILExpressionNode Expression { get; }

        public ASTCILIsVoidNode( ASTCILExpressionNode expression) : base()
        {
            Expression = expression;
        }

        public override Result Accept<Result>( ICILVisitor<Result> Visitor ) => Visitor.VisitIsVoid( this );
    }
}
