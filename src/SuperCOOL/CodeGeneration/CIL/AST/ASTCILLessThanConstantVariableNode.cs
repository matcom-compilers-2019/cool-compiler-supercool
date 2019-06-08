﻿using SuperCOOL.Constants;

namespace SuperCOOL.CodeGeneration.CIL.AST
{
    class ASTCILLessThanConstantVariableNode : ASTCILExpressionNode
    {
        public int Left { get; }
        public ASTCILExpressionNode Right { get; }

        public ASTCILLessThanConstantVariableNode(int left, ASTCILExpressionNode right) : base(Types.Bool)
        {
            Left = left;
            Right = right;
        }
    }
}
