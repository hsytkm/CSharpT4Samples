using FluentAssertions;
using System;
using Xunit;

namespace CSharpT4Samples.Tests
{
    public class MainTest
    {
        [Fact]
        public void ReadDataTest()
        {
            static int GetSliceStartIndex(int itemIndex, int atsvIndex) =>
                sizeof(Int16) * (3 * itemIndex + (atsvIndex % 3));

            var data = BinarySupplier.GetPlineData();
            var span = new Span<byte>(data.Buffer);
            var pline = new ProgramLine(data.Length, data.FixedValue, span);

            var apex0av = BitConverter.ToInt16(span.Slice(GetSliceStartIndex(0, 0)));
            pline.Apexs[0].Av.Should().Be(apex0av);

            var apex0tv = BitConverter.ToInt16(span.Slice(GetSliceStartIndex(0, 1)));
            pline.Apexs[0].Tv.Should().Be(apex0tv);

            var apex0sv = BitConverter.ToInt16(span.Slice(GetSliceStartIndex(0, 2)));
            pline.Apexs[0].Sv.Should().Be(apex0sv);

            var apex2av = BitConverter.ToInt16(span.Slice(GetSliceStartIndex(2, 0)));
            pline.Apexs[2].Av.Should().Be(apex2av);

            var apex3tv = BitConverter.ToInt16(span.Slice(GetSliceStartIndex(3, 1)));
            pline.Apexs[3].Tv.Should().Be(apex3tv);

            var apex4sv = BitConverter.ToInt16(span.Slice(GetSliceStartIndex(4, 2)));
            pline.Apexs[4].Sv.Should().Be(apex4sv);
        }

    }
}
