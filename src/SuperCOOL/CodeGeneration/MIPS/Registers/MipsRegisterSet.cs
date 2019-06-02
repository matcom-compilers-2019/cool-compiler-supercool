﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SuperCOOL.CodeGeneration.MIPS.Registers
{
    public static class MipsRegisterSet
    {
        // Stack pointer
        public static readonly StackPointer sp = new StackPointer( "$sp" );

        // Temporary registers
        public static readonly TemporaryRegister t0 = new TemporaryRegister( "$t0" );
        public static readonly TemporaryRegister t1 = new TemporaryRegister( "$t1" );

        // Argument registers
        public static readonly ArgumentRegister a0 = new ArgumentRegister( "$a0" );

        // Value registers
        public static readonly ValueRegister v0 = new ValueRegister( "$v0" );
    }
}