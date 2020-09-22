using System;
using System.Runtime.InteropServices;

namespace CSharpT4Samples
{
    [StructLayout(LayoutKind.Sequential, Size = 6)]
    readonly struct ApexAtsv
    {
        public readonly Int16 Av;
        public readonly Int16 Tv;
        public readonly Int16 Sv;
    }

    //class ApexAtsvActual
    //{
    //    public double Av { get; }
    //    public double Tv { get; }
    //    public double Sv { get; }
    //    public ApexAtsvActual(in ApexAtsv apex, int fixedNumber)
    //    {
    //        if (fixedNumber <= 0) throw new DivideByZeroException();
    //        Av = apex.Av / (double)fixedNumber;
    //        Tv = apex.Tv / (double)fixedNumber;
    //        Sv = apex.Sv / (double)fixedNumber;
    //    }
    //}
}
