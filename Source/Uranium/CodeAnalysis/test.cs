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
                10 => (int)TestTwo(),
                _ => TestThree(),
            };
        private static int TestTwo()
            => 2;
        private static int TestThree()
            => 3;
    }
}
