﻿using System.Runtime.InteropServices.ComTypes;
using SuperCOOL.Core;

namespace SuperCOOL.CodeGeneration.CIL.AST
{
    public class ASTCILRuntimeErrorNode : ASTCILExpressionNode
    {
        public int Id { get; }
        public ASTCILRuntimeErrorNode( int id, ISymbolTable symbolTable) : base(symbolTable)
        {
            Id = id;
        }

        public override Result Accept<Result>( ICILVisitor<Result> Visitor ) => Visitor.VisitRuntimeError( this );
    }
}
