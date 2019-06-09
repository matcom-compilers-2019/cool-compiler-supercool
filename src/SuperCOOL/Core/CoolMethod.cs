﻿using System;
using System.Collections.Generic;
using SuperCOOL.SemanticCheck;
using SuperCOOL.SemanticCheck.AST;

namespace SuperCOOL.Core
{
    public class CoolMethod 
    {
        public CoolMethod(CoolType contextType,string name, List<CoolType> formals, CoolType returnType)
        {
            Type = contextType;
            Name = name;
            Params = formals;
            ReturnType = returnType;
        }

        public CoolType Type { get;}

        List<CoolType> Params;
        public string Name { get; }
        public CoolType ReturnType { get; }

        public virtual CoolType GetParam(int i)
        {
            return Params[i];
        }

        public virtual bool EnsureParametersCount(int length)
        {
            return Params.Count == length;
        }

        public virtual bool EnsureParameter(int index, CoolType type)
        {
            return type.IsIt(Params[index]);
        }

        public int CountParams {get;set;}
    }

    public class NullMethod : CoolMethod
    {
        public NullMethod(string name):base(null,name,new List<CoolType>(),new NullType())
        {
        }
        public override bool EnsureParameter(int index, CoolType type)
        {
            return true;
        }

        public override bool EnsureParametersCount(int length)
        {
            return true;
        }

        public override CoolType GetParam(int i)
        {
            return new NullType();
        }
    }

}