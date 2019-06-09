﻿namespace SuperCOOL.CodeGeneration.CIL.AST
{
    public class ASTCILDivideConstantVariableNode : ASTCILExpressionNode
    {
        public int Left { get; }
        public ASTCILExpressionNode Right { get; }

        public ASTCILDivideConstantVariableNode( int left, ASTCILExpressionNode right, Core.ISymbolTable symbolTable):base(symbolTable)
        {
            Left = left;
            Right = right;
        }

        public override Result Accept<Result>( ICILVisitor<Result> Visitor ) => Visitor.VisitDivideConstantVariable( this );
    }
}
