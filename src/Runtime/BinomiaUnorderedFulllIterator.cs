using System;
using System.Collections.Generic;

namespace BackPackOptimizer.Runtime
{
    public sealed class BinomiaUnorderedFulllIterator<T>
    {
        readonly T[] _array;

        public BinomiaUnorderedFulllIterator(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            _array = array;
        }

        public long TotalCombinationsCount => UnorderedFullPickSize(_array.Length);

        public IEnumerable<int[]> Iterate()
        {
            int n = _array.Length;

            for (int num = 1; num <= n; ++num) //taking N elements from M
            {
                int[] indexes = new int[num];

                int level = 0;
                bool down = true;
                while (indexes[0] < n - num + 1)
                {
                    if (down)
                    {
                        if (level > 0)
                            indexes[level] = indexes[level - 1] + 1;

                        if (level == num - 1)
                        {
                            while (indexes[level] < n)
                            {
                                yield return indexes;
                                indexes[level]++;
                            }

                            if (num > 1)
                                level--;

                            down = false;
                        }
                        else
                            level++;
                    }
                    else {
                        indexes[level]++;

                        if (indexes[level] <= n - num + level)
                        {
                            down = true;
                            if (num > 0)
                                level++;
                        }
                        else
                            level--;
                    }
                }
            }
        }

        public static long UnorderedFullPickSize(int n)
        {
            long res = 0;
            long nFac = Factorial(n);
            for (int m = 1; m <= n; ++m)
                res += nFac / (Factorial(m) * Factorial(n - m));

            return res;
        }
    
        public static long Factorial(int i)
            {
                if (i < 0)
                    return -1L;
                if (i <= 1)
                    return 1L;

                long res = 1;
                checked
                {
                    for (long k = 2; k <= i; k++)
                        res *= k;
                }

            return res;
            }
        }
}
