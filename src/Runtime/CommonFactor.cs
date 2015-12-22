using System;
using System.Collections.Generic;

namespace BackPackOptimizer.Runtime
{
    public static class CommonFactor
    {
        public static long Calculate (long a, long b)
        {
            if (a < b)
            {
                a ^= b;
                b ^= a;
                a ^= b;
            }

            return b == 0 ? a : Calculate(b, a % b);
        }

        public static long Calculate<T> (IList<T> array, Func<T, long> getterVal, int n)
        {
            if (n == 1)
                return getterVal(array[ 0 ]);

            return Calculate(getterVal(array[n - 1]), Calculate(array, getterVal, n - 1));
        }
    }
}
