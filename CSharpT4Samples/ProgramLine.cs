using CSharpT4Samples.T4;
using System;
using System.Runtime.CompilerServices;

namespace CSharpT4Samples
{
    /*record*/
    readonly struct ProgramLine
    {
        public readonly int Length;         // Apexの使用数
        public readonly int FixedNumber;
        public readonly PlineApexs Apexs;   // Apexの最大定義数

        public ProgramLine(int length, int fixedNumber, Span<byte> span)
        {
            Length = length;
            FixedNumber = fixedNumber;
            Apexs = Unsafe.As<byte, PlineApexs>(ref span.GetPinnableReference());
        }
    }
}
