using CSharpT4Samples.T4;
using FluentAssertions;
using System;
using System.Runtime.InteropServices;
using Xunit;

namespace CSharpT4Samples.Tests
{
    public class BinarySupplierTest
    {
        [Fact]
        public void DataTest()
        {
            var data = BinarySupplier.GetPlineData();

            // Length(Apexの使用数)が予約数より小さいこと
            var plineSize = Marshal.SizeOf(typeof(PlineApexs));
            data.Length.Should().BeGreaterThan(0);
            data.Length.Should().BeLessOrEqualTo(plineSize);

            // 固定小数点が正数
            data.FixedValue.Should().BeGreaterThan(0);

            // buffer(Apexのbyte[])が仕様通り(予約通り)であること
            var apexSize = Marshal.SizeOf(typeof(ApexAtsv));
            var apexCount = new PlineApexs().Length;
            data.Buffer.Length.Should().Be(apexSize * apexCount);
        }

    }
}
