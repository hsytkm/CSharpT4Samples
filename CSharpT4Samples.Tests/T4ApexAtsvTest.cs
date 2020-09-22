using CSharpT4Samples.T4;
using FluentAssertions;
using System;
using System.Runtime.InteropServices;
using Xunit;

namespace CSharpT4Samples.Tests
{
    public class T4ApexAtsvTest
    {
        [Fact]
        public void SizeOf()
        {
            var apexSize = Marshal.SizeOf(typeof(ApexAtsv));
            apexSize.Should().Be(6);

            var plineSize = Marshal.SizeOf(typeof(PlineApexs));
            plineSize.Should().Be(apexSize * new PlineApexs().Length);
        }

        [Fact]
        public void Indexer()
        {
            var apexs = new PlineApexs();
            var atsvZero = new ApexAtsv();

            for (int i = 0; i < apexs.Length; i++)
            {
                apexs[i].Should().Be(atsvZero);
            }
        }

        [Fact]
        public void Foreach()
        {
            var apexs = new PlineApexs();
            var atsvZero = new ApexAtsv();

            foreach (var apex in apexs)
            {
                apex.Should().Be(atsvZero);
            }
        }
    }
}
