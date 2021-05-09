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
            Assert.Equal(10, a);
        }
    }
}
