﻿using SuperCOOL.Core;

namespace SuperCOOL.CodeGeneration.CIL.AST
{
    public class ASTCILSetAttributeNode : ASTCILExpressionNode
    {
        public string TypeName { get; }
        public string AttributeName { get; }
        public ASTCILExpressionNode Expression { get; }

        public ASTCILSetAttributeNode( string typeName, string attributeName, ASTCILExpressionNode expression,ISymbolTable symbolTable):base(symbolTable)
        {
            TypeName = typeName;
            AttributeName = attributeName;
            Expression = expression;
        }

        public override Result Accept<Result>( ICILVisitor<Result> Visitor ) => Visitor.VisitSetAttribute( this );
    }
}
