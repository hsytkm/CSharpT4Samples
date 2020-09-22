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

            // Length(Apex�̎g�p��)���\�񐔂�菬��������
            var plineSize = Marshal.SizeOf(typeof(PlineApexs));
            data.Length.Should().BeGreaterThan(0);
            data.Length.Should().BeLessOrEqualTo(plineSize);

            // �Œ菬���_������
            data.FixedValue.Should().BeGreaterThan(0);

            // buffer(Apex��byte[])���d�l�ʂ�(�\��ʂ�)�ł��邱��
            var apexSize = Marshal.SizeOf(typeof(ApexAtsv));
            var apexCount = new PlineApexs().Length;
            data.Buffer.Length.Should().Be(apexSize * apexCount);
        }

    }
}
