﻿using System;
using System.Collections.Generic;
using System.Text;
using SuperCOOL.Constants;
using SuperCOOL.Core;
using SuperCOOL.SemanticCheck.AST;

namespace SuperCOOL.SemanticCheck
{
    public class SuperCoolTypeDefsVisitor : SuperCoolASTVisitor<SemanticCheckResult>
    {
        private CompilationUnit CompilationUnit { get; set; }
        public SuperCoolTypeDefsVisitor(CompilationUnit compilationUnit)
        {
            this.CompilationUnit = compilationUnit;
        }

        public override SemanticCheckResult VisitProgram(ASTProgramNode Program)
        {
            //Creating All Types
            foreach (var type in Program.Clases)
            {
                var exist = CompilationUnit.TypeEnvironment.GetTypeDefinition(type.TypeName,Program.SymbolTable,out var _);
                Program.SemanticCheckResult.Ensure(!exist, 
                    new Lazy<Error>(()=>new Error($"Multiple Definitions for class {type.TypeName}", ErrorKind.TypeError,type.Type.Line,type.Type.Column)));
                if (!exist)
                {
                    CompilationUnit.TypeEnvironment.AddType(type.SymbolTable);
                    CompilationUnit.TypeEnvironment.GetTypeDefinition(type.TypeName, Program.SymbolTable, out var coolType);
                    CompilationUnit.MethodEnvironment.AddMethod(coolType,Functions.Init, new List<CoolType>(), coolType,new SymbolTable(coolType.SymbolTable));
                    var init=CompilationUnit.MethodEnvironment.GetMethod(coolType, Functions.Init);
                    init.AssignParametersAndLocals();
                }
            }

            //Inheritance
            foreach (var type in Program.Clases)
                Program.SemanticCheckResult.Ensure(type.Accept(this));

            Program.SemanticCheckResult.Ensure(CompilationUnit.NotCyclicalInheritance(), 
                new Lazy<Error>(()=>new Error("Detected Cyclical Inheritance", ErrorKind.SemanticError)));

            return Program.SemanticCheckResult;
        }

        public override SemanticCheckResult VisitClass(ASTClassNode Class)
        {
            var exist = CompilationUnit.TypeEnvironment.GetTypeDefinition(Class.ParentTypeName,Class.SymbolTable,out var _);
            Class.SemanticCheckResult.Ensure(exist, new Lazy<Error>(()=>new Error($"Missing declaration for type {Class.ParentTypeName}.", ErrorKind.TypeError,Class.ParentType.Line,Class.ParentType.Column)));
            if (exist)
                CompilationUnit.TypeEnvironment.AddInheritance(Class.TypeName, Class.ParentTypeName);
            return Class.SemanticCheckResult;
        }

    }
}
