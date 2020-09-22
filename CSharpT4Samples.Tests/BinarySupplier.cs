using System;

namespace CSharpT4Samples.Tests
{
    static class BinarySupplier
    {
        private const int EV_KEISU = 256;
        private static short ToEvKeisu(int value) => (short)(value * EV_KEISU);

        private static void SetApex(ref Int16[] bin, int offset, Int16 av, Int16 tv, Int16 sv)
        {
            bin[0 + offset] = av;
            bin[1 + offset] = tv;
            bin[2 + offset] = sv;
        }

        public static (int Length, int FixedValue, byte[] Buffer) GetPlineData() =>
            (5, EV_KEISU, GetPlineBytes());

        private static byte[] GetPlineBytes()
        {
            var ary = new Int16[3/*sizeof(Apex)*/ * 32/*Items*/];

            SetApex(ref ary, 0,  ToEvKeisu(3), 0, ToEvKeisu(5));
            SetApex(ref ary, 3,  ToEvKeisu(3), ToEvKeisu(6), ToEvKeisu(5));
            SetApex(ref ary, 6,  ToEvKeisu(3), ToEvKeisu(6), 0);
            SetApex(ref ary, 9,  ToEvKeisu(8), ToEvKeisu(6), 0);
            SetApex(ref ary, 12, ToEvKeisu(8), ToEvKeisu(12), 0);

            return ToArrayInt16ToSpanByte(ary);

            static byte[] ToArrayInt16ToSpanByte(Int16[] source)
            {
                var bytes = new byte[source.Length * sizeof(Int16)];
                for (int i = 0; i < source.Length; ++i)
                {
                    var bs = BitConverter.GetBytes(source[i]);
                    for (int j = 0; j < bs.Length; ++j)
                    {
                        bytes[i * sizeof(Int16) + j] = bs[j];
                    }
                }
                return bytes;
            }
        }

    }
}
