using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis
{
    public static class Test
    {
        public static int TestOne(int a)
            => a switch
            {
                _ => testTwo(),
            };
        private static int testTwo()
            => 10;
    }
}
