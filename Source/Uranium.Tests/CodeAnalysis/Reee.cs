using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis;
using Xunit;

namespace Uranium.Tests.CodeAnalysis
{
    public static class Reee
    {
        [Fact]
        public static void ABc()
        {
            var a = Test.TestOne(10);
            Assert.Equal(2, a);
            var b = Test.TestOne(11);
            Assert.Equal(3, b);
        }
    }
}
