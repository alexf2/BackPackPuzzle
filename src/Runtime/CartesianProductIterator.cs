using System;
using System.Collections.Generic;

namespace BackPackOptimizer.Runtime
{
    public sealed class CartesianProductIterator<T>
    {
        T[][] _arrays;

        public CartesianProductIterator(T[][] arrays)
        {
            if (arrays == null)
                throw new ArgumentNullException(nameof(arrays));
            if (arrays.Length < 2)
                throw new ArgumentException("To calculate Cartesian product there should be at least two arrays");

            _arrays = arrays;
        }

        public long TotalCombinationsCount  {
            get {
                int res = 1;
		        foreach (var arr in _arrays)
			        res *= arr.Length;
		        return res;
            }
        }

        public IEnumerable<int[]> Iterate()
        {
            int[] index = new int[ _arrays.Length ];
            int n = _arrays.Length;
            int lastIdx = n - 1;

            while (index[0] < _arrays[0].Length)
            {
                while (index[lastIdx] < _arrays[lastIdx].Length)
                {
                    yield return index;
                    index[ lastIdx ]++;
                }

                index[ lastIdx ] = 0;
                int j = n - 2;
                while (j > 0)
                {
                    index[ j ]++;
                    if (index[j] < _arrays[j].Length)
                        break;

                    index[ j-- ] = 0;
                }

                if (j == 0)
                    index[ 0 ]++;
            }
        }
    }
}
