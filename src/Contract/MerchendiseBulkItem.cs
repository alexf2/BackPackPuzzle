using System;
using System.Collections.Generic;
using System.Linq;
using DataProviders.Contract;

namespace BackPackOptimizer.Contract
{
    public sealed class MerchendiseBulkItem
    {
        readonly Merchendise _m;

        public MerchendiseBulkItem (Merchendise m)
        {
            _m = m;
        }

        public Merchendise Merchendise => _m;

        int? _subItemsCount;
        public int SubItemsCount => _subItemsCount ?? (_subItemsCount = (_m.Size - _m.MinSize)/_m.IncrementStep + 1).Value;

        public int GetNthVolumeGallons(int i)
        {
            if (i < 0 || i > SubItemsCount - 1)
                throw new ArgumentException($"Subitem index is not in range: {0} - {SubItemsCount - 1}");

            return _m.MinSize + _m.IncrementStep * i;
        }

        public double GetNthPrice(int i)
        {
            return GetNthVolumeGallons(i) * _m.AvgPrice;
        }

        public static MerchendiseBulkItem[] ToBulkItems(IEnumerable<Merchendise> m) => m.Select(item => new MerchendiseBulkItem(item)).ToArray();

        public static void CartesianProductIteration(MerchendiseBulkItem[] arr, int[] indexes, Func<int[], bool> callback)
        {            
            int[] index = new int[indexes.Length];
            int n = indexes.Length; //the number of arrays to product
            int lastIdx = n - 1;

            if (indexes.Length < 2)
            {
                var m = arr[indexes[ 0 ]];
                int subItemsCnt = m.SubItemsCount;

                for (int i = 0; i < subItemsCnt; ++i)
                {
                    index[0] = i;
                    if (!callback(index))
                        break;
                }
                return;
            }

            while (index[0] < arr[indexes[0]].SubItemsCount)
            {
                while (index[lastIdx] < arr[indexes[lastIdx]].SubItemsCount)
                {
                    if (!callback(index))
                        return;
                    index[lastIdx]++;
                }

                index[lastIdx] = 0;
                int j = n - 2;
                while (j > 0)
                {
                    index[j]++;
                    if (index[j] < arr[indexes[j]].SubItemsCount)
                        break;

                    index[j--] = 0;
                }

                if (j == 0)
                    index[0]++;
            }
        }
    }
}
