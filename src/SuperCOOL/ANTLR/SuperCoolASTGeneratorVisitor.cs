﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using SuperCOOL.ANTLR;
using SuperCOOL.Core;
using SuperCOOL.SemanticCheck.AST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperCOOL.ANTLR
{
    public class SuperCoolASTGeneratorVisitor : SuperCOOLBaseVisitor<ASTNode>
    {
        SymbolTable CurrentTable;

        public SuperCoolASTGeneratorVisitor()
        {
            CurrentTable = new SymbolTable();
        }

        public override ASTNode VisitAddminus([NotNull] SuperCOOLParser.AddminusContext context)
        {
            if (context.ADD() != null)
            {
                var resultadd=new ASTAddNode() { };
                ASTExpressionNode leftadd = (ASTExpressionNode)context.expression()[0].Accept(this);
                AssignSymbolTable(leftadd);
                ASTExpressionNode rightadd = (ASTExpressionNode)context.expression()[1].Accept(this);
                AssignSymbolTable(rightadd);
                resultadd.Left = leftadd;
                resultadd.Right = rightadd;
                resultadd.AddToken = context.ADD().Symbol;
                return resultadd;
            }
            var result=new ASTMinusNode() { };
            ASTExpressionNode left = (ASTExpressionNode)context.expression()[0].Accept(this);
            AssignSymbolTable(left);
            ASTExpressionNode right = (ASTExpressionNode)context.expression()[1].Accept(this);
            AssignSymbolTable(right);

            result.Left = left;
            result.Right = right;
            result.MinusToken = context.MINUS().Symbol;
            return result;
            
        }

        public override ASTNode VisitAssignment([NotNull] SuperCOOLParser.AssignmentContext context)
        {
            var result = new ASTAssingmentNode();
            ASTExpressionNode exp = (ASTExpressionNode)context.expression().Accept(this);
            AssignSymbolTable(exp);
            ASTIdNode id = new ASTIdNode() { Token = context.OBJECTID().Symbol};
            AssignSymbolTable(id);
            result.Id = id;
            result.Expresion = exp;
            result.AssigmentToken = context.ASSIGNMENT().Symbol;
            return result;
        }

        public override ASTNode VisitBlock([NotNull] SuperCOOLParser.BlockContext context)
        {
            var result = new ASTBlockNode();
            var exps = context.expression();
            ASTExpressionNode[] exp=new ASTExpressionNode[exps.Length];
            for (int i = 0; i < exps.Length; i++)
            {
                exp[i] = (ASTExpressionNode)exps[i].Accept(this);
                AssignSymbolTable(exp[i]);
            }
            result.Expresions = exp;
            return result;
        }

        public override ASTNode VisitBoolNot([NotNull] SuperCOOLParser.BoolNotContext context)
        {
            var result = new ASTBoolNotNode();
            var expresion=(ASTExpressionNode)context.expression().Accept(this);
            AssignSymbolTable(expresion);
            result.Expresion = expresion;
            result.NotToken = context.NOT().Symbol;
            return result;
        }

        public override ASTNode VisitCase([NotNull] SuperCOOLParser.CaseContext context)
        {
            var result = new ASTCaseNode();
            var expCases = (ASTExpressionNode)context.expression()[0].Accept(this);
            AssignSymbolTable(expCases);
            var cases = new (IToken, IToken, ASTExpressionNode)[context.expression().Length-1];
            for (int i = 0; i < cases.Length; i++)
            {
                var objectName = context.OBJECTID(i).Symbol;
                var typename = context.TYPEID(i).Symbol;
                EnterScope();
                CurrentTable.DefObject(objectName.Text, typename.Text,ObjectKind.Local);
                cases[i] = (objectName,typename,(ASTExpressionNode)context.expression(i+1).Accept(this));
                AssignSymbolTable(cases[i].Item3);
                ExitScope();
            }
            result.ExpressionCase = expCases;
            result.Cases = cases;
            return result;
        }

        public override ASTNode VisitClassDefine([NotNull] SuperCOOLParser.ClassDefineContext context)
        {
            var className = context.TYPEID(0).Symbol.Text;
            var result = new ASTClassNode();
            var methods = new List<ASTMethodNode>();
            var atributes = new List<ASTAtributeNode>();
            CurrentTable.DefObject("self", "SELF_TYPE",ObjectKind.Self);
            CurrentTable.DefObject("_self", className,ObjectKind.ContextType);

            foreach (var item in context.feature().OfType<SuperCOOLParser.PropertyContext>())
                CurrentTable.DefObject(item.OBJECTID().Symbol.Text, item.TYPEID().Symbol.Text, ObjectKind.Atribute);

            var _init = new ASTMethodNode();
            methods.Add(_init);
            _init.Method = new CommonToken(-1, Constants.Functions.Init);
            _init.Return =new CommonToken(context.TYPEID(0).Symbol.Type,className);
            _init.Formals=new List<(IToken name, IToken type)>();
            _init.Body=new ASTBlockNode(){Expresions = new ASTExpressionNode[] { }};
            EnterScope();//_init symbol table
            foreach (var item in context.feature().OfType<SuperCOOLParser.PropertyContext>())
            {
                var atribute = (ASTAtributeNode)item.Accept(this);
                atributes.Add(atribute);
                AssignSymbolTable(atribute);
            }
            AssignSymbolTable(_init);
            ExitScope();

            foreach (var item in context.feature().OfType<SuperCOOLParser.MethodContext>())
            {
                EnterScope();
                var method = (ASTMethodNode)item.Accept(this);
                methods.Add(method);
                AssignSymbolTable(method);
                ExitScope();
            }

            result.Type = context.TYPEID(0).Symbol;
            result.ParentType = context.TYPEID(1)?.Symbol??null;
            result.Methods = methods;
            result.Atributes = atributes;
            return result;
        }


        public override ASTNode VisitComparison([NotNull] SuperCOOLParser.ComparisonContext context)
        {
            if (context.EQUAL() != null)
            {
                var resultequal = new ASTEqualNode();
                ASTExpressionNode leftequal = (ASTExpressionNode)context.expression()[0].Accept(this);
                AssignSymbolTable(leftequal);
                ASTExpressionNode rightequal = (ASTExpressionNode)context.expression()[1].Accept(this);
                AssignSymbolTable(rightequal);

                resultequal.Left = leftequal;
                resultequal.Right = rightequal;
                resultequal.EqualToken = context.EQUAL().Symbol;
                return resultequal;
            }
            if (context.LESS_THAN() != null)
            {
                var resultless = new ASTLessThanNode();
                ASTExpressionNode leftless = (ASTExpressionNode)context.expression()[0].Accept(this);
                AssignSymbolTable(leftless);
                ASTExpressionNode rightless = (ASTExpressionNode)context.expression()[1].Accept(this);
                AssignSymbolTable(rightless);

                resultless.Left = leftless;
                resultless.Right = rightless;
                resultless.LessThanToken = context.LESS_THAN().Symbol;
                return resultless;
            }
            var result = new ASTLessEqualNode();
            ASTExpressionNode left = (ASTExpressionNode)context.expression()[0].Accept(this);
            AssignSymbolTable(left);
            ASTExpressionNode right = (ASTExpressionNode)context.expression()[1].Accept(this);
            AssignSymbolTable(right);

            result.Left = left;
            result.Right = right;
            result.LessEqualToken = context.LESS_EQUAL().Symbol;
            return result;
        }

        public override ASTNode VisitFalse([NotNull] SuperCOOLParser.FalseContext context)
        {
            return new ASTBoolConstantNode() {Value=false };
        }

        public override ASTNode VisitIf([NotNull] SuperCOOLParser.IfContext context)
        {
            var result = new ASTIfNode();
            var cond = (ASTExpressionNode)context.expression(0).Accept(this);
            AssignSymbolTable(cond);
            var then = (ASTExpressionNode)context.expression(1).Accept(this);
            AssignSymbolTable(then);
            var @else = (ASTExpressionNode)context.expression(2).Accept(this);
            AssignSymbolTable(@else);

            result.Condition = cond;
            result.Then = then;
            result.Else = @else;
            result.IfToken = context.IF().Symbol;
            return result;
        }

        public override ASTNode VisitInt([NotNull] SuperCOOLParser.IntContext context)
        {
            var value = int.Parse(context.INT().Symbol.Text);
            return new ASTIntConstantNode() {Value=value };
        }

        public override ASTNode VisitIsvoid([NotNull] SuperCOOLParser.IsvoidContext context)
        {
            var result = new ASTIsVoidNode();
            var expression = context.expression().Accept(this);
            AssignSymbolTable(expression);

            result.Expression = (ASTExpressionNode) expression;
            return result;
        }

        public override ASTNode VisitLetIn([NotNull] SuperCOOLParser.LetInContext context)
        {
            var result = new ASTLetInNode();
            var declarations = new (IToken, IToken, ASTExpressionNode)[context.letassign().Length];
            for (int i = 0; i < declarations.Length; i++)
            {
                declarations[i] = (context.letassign(i).OBJECTID()?.Symbol,context.letassign(i).TYPEID().Symbol,(ASTExpressionNode)context.letassign(i).expression()?.Accept(this));
                if(declarations[i].Item3!=null)
                    AssignSymbolTable(declarations[i].Item3);
            }

            EnterScope();
            foreach (var declaration in declarations)
                CurrentTable.DefObject(declaration.Item1.Text,declaration.Item2.Text,ObjectKind.Local);

            var letExp = (ASTExpressionNode)context.expression().Accept(this);
            AssignSymbolTable(letExp);
            ExitScope();

            result.Declarations = declarations.Select(x=>(letExp.SymbolTable.GetObject(x.Item1.Text),x.Item2,x.Item3)).ToArray();
            result.LetExp = letExp;
            return result;
        }

        public override ASTNode VisitMethod([NotNull] SuperCOOLParser.MethodContext context)
        {
            var result = new ASTMethodNode();
            var formals = context.formal().Select(x=>(x.OBJECTID().Symbol,x.TYPEID().Symbol)).ToList();
            foreach (var item in formals)
                CurrentTable.DefObject(item.Item1.Text,item.Item2.Text,ObjectKind.Parameter);

            var body = (ASTExpressionNode)context.expression().Accept(this);
            AssignSymbolTable(body);

            result.Method = context.OBJECTID().Symbol;
            result.Body = body;
            result.Return = context.TYPEID().Symbol;
            result.Formals = formals;
            return result;
        }

        public override ASTNode VisitMethodCall([NotNull] SuperCOOLParser.MethodCallContext context)
        {
            if (context.TYPEID() != null)
            {
                var result = new ASTStaticMethodCallNode();
                var expresions = context.expression();
                var invokeOnExpresion = (ASTExpressionNode)expresions[0].Accept(this);
                AssignSymbolTable(invokeOnExpresion);
                var methodName = context.OBJECTID().Symbol;
                var arguments = new ASTExpressionNode[expresions.Length - 1];
                for (int i = 0; i < arguments.Length; i++)
                {
                    arguments[i] = (ASTExpressionNode)expresions[i+1].Accept(this);
                    AssignSymbolTable(arguments[i]);
                }
                var type = context.TYPEID().Symbol;
                result.Method = methodName;
                result.Type= type;
                result.InvokeOnExpresion = invokeOnExpresion;
                result.Arguments = arguments;

                return result;
            }
            var dresult = new ASTDynamicMethodCallNode();
            var dexpresions = context.expression();
            var dinvokeOnExpresion = (ASTExpressionNode)dexpresions[0].Accept(this);
            AssignSymbolTable(dinvokeOnExpresion);
            var dmethodName = context.OBJECTID().Symbol;
            var darguments = new ASTExpressionNode[dexpresions.Length - 1];
            for (int i = 0; i < darguments.Length; i++)
            {
                darguments[i] = (ASTExpressionNode)dexpresions[i+1].Accept(this);
                AssignSymbolTable(darguments[i]);
            }
            dresult.Method = dmethodName;
            dresult.InvokeOnExpresion = dinvokeOnExpresion;
            dresult.Arguments = darguments;

            return dresult;
        }

        public override ASTNode VisitMultiplydivision([NotNull] SuperCOOLParser.MultiplydivisionContext context)
        {
            if (context.MULTIPLY()!=null)
            {
                var resultmult = new ASTMultiplyNode();
                ASTExpressionNode leftmult = (ASTExpressionNode)context.expression()[0].Accept(this);
                AssignSymbolTable(leftmult);
                ASTExpressionNode rightmult = (ASTExpressionNode)context.expression()[1].Accept(this);
                AssignSymbolTable(rightmult);

                resultmult.Left = leftmult;
                resultmult.Right = rightmult;
                resultmult.MultToken = context.MULTIPLY().Symbol;
                return resultmult;
            }

            var result = new ASTDivideNode();
            ASTExpressionNode left = (ASTExpressionNode)context.expression()[0].Accept(this);
            AssignSymbolTable(left);
            ASTExpressionNode right = (ASTExpressionNode)context.expression()[1].Accept(this);
            AssignSymbolTable(right);

            result.Left = left;
            result.Right = right;
            result.DivToken = context.DIVISION().Symbol;
            return result;
        }

        public override ASTNode VisitNegative([NotNull] SuperCOOLParser.NegativeContext context)
        {
            var result=new ASTNegativeNode();
            var exp = context.expression().Accept(this);
            AssignSymbolTable(exp);
            result.Expression =exp;
            result.NegativeToken = context.INTEGER_NEGATIVE().Symbol;
            return result;
        }

        public override ASTNode VisitNew([NotNull] SuperCOOLParser.NewContext context)
        {
            return new ASTNewNode() { Type = context.TYPEID().Symbol };
        }

        public override ASTNode VisitOwnMethodCall([NotNull] SuperCOOLParser.OwnMethodCallContext context)
        {
            var result = new ASTOwnMethodCallNode();

            var exps = context.expression();
            var arguments = new ASTExpressionNode[exps.Length];
            for (int i = 0; i < exps.Length; i++)
            {
                arguments[i] = (ASTExpressionNode)exps[i].Accept(this);
                AssignSymbolTable(arguments[i]);
            }

            result.Method = context.OBJECTID().Symbol;
            result.Arguments=arguments ;
            return result;
        }

        public override ASTNode VisitParentheses([NotNull] SuperCOOLParser.ParenthesesContext context)
        {
            return context.expression().Accept(this);
        }
        public override ASTNode VisitClasses([NotNull] SuperCOOLParser.ClassesContext context)
        {
            var program =new ASTProgramNode();
            var clases = context.classDefine();
            var ProgramTable = new SymbolTable();
            foreach (var item in clases)
            {
                EnterScope();
                var classe = (ASTClassNode)item.Accept(this);
                AssignSymbolTable(classe);
                program.Clases.Add(classe);
                ExitScope();
            }

            AssignSymbolTable(program);
            return program;
        }

        public override ASTNode VisitEof([NotNull] SuperCOOLParser.EofContext context)
        {
            var prog= new ASTProgramNode();
            AssignSymbolTable(prog);
            return prog;
        }

        public override ASTNode VisitProperty([NotNull] SuperCOOLParser.PropertyContext context)
        {
            var result = new ASTAtributeNode();
            var init = (ASTExpressionNode)context.expression()?.Accept(this)??null;
            if(init!=null)
                AssignSymbolTable(init);

            result.Attribute = context.OBJECTID().Symbol;
            result.Type = context.TYPEID().Symbol;
            result.Init = init;

            return result;
        }

        public override ASTNode VisitString([NotNull] SuperCOOLParser.StringContext context)
        {
            return new ASTStringConstantNode() { Value = context.STRING().Symbol.Text.Trim('"') };
        }

        public override ASTNode VisitTrue([NotNull] SuperCOOLParser.TrueContext context)
        {
            return new ASTBoolConstantNode() { Value = true };
        }

        public override ASTNode VisitWhile([NotNull] SuperCOOLParser.WhileContext context)
        {
            var result = new ASTWhileNode();
            var condition = context.expression(0).Accept(this);
            AssignSymbolTable(condition);
            var body = context.expression(1).Accept(this);
            AssignSymbolTable(body);

            result.Condition = (ASTExpressionNode) condition;
            result.Body = (ASTExpressionNode) body;
            return result;
        }

        public override ASTNode VisitId([NotNull] SuperCOOLParser.IdContext context)
        {
            return new ASTIdNode() { Token = context.OBJECTID().Symbol };
        }

        private void AssignSymbolTable(ASTNode node)
        {
            node.SymbolTable = CurrentTable;
        }

        private void EnterScope()
        {
            CurrentTable =(SymbolTable)CurrentTable.EnterScope();
        }

        private void ExitScope()
        {
            CurrentTable = (SymbolTable)CurrentTable.ExitScope();
        }

    }
}
