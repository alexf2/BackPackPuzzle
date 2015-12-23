using System;
using System.Collections.Generic;
using System.Linq;
using DataProviders.Contract;

namespace BackPackOptimizer.Contract
{
    /// <summary>
    /// Represents a decorator for a merchendise item. Provides integer optimization specific API and helper methods to iterate subitems (selecting multiple items).
    /// </summary>
    public sealed class MerchendiseBulkItem
    {
        readonly Merchendise _m;

        /// <summary>
        /// Initializaes a new instance.
        /// </summary>
        /// <param name="m"></param>
        public MerchendiseBulkItem (Merchendise m)
        {
            _m = m;
        }

        
        /// <summary>
        /// Returns underlying merchecndise item.
        /// </summary>
        public Merchendise Merchendise => _m;

        int? _subItemsCount;
        /// <summary>
        /// Return total count of items, which can be selected, taking in account MinSize, Size and Step.
        /// </summary>
        public int SubItemsCount => _subItemsCount ?? (_subItemsCount = (_m.Size - _m.MinSize)/_m.IncrementStep + 1).Value;

        /// <summary>
        /// Helper to decorate base merchendise.
        /// </summary>
        /// <param name="m">Merchendise collection.</param>
        /// <returns>Decorated items array.</returns>
        public static MerchendiseBulkItem[] ToBulkItems(IEnumerable<Merchendise> m) => m.Select(item => new MerchendiseBulkItem(item)).ToArray();

        /// <summary>
        /// Calculates volume/weight of I possible selection.
        /// </summary>
        /// <param name="i">Selection index between 0 and SubItemsCount - 1. 0 - means, that we select MinSize of items.</param>
        /// <returns>Total volume of selected merchendise items.</returns>
        public int GetNthVolumeGallons(int i)
        {
            if (i == -1)
                return -1;

            if (i < 0 || i > SubItemsCount - 1)
                throw new ArgumentException($"Subitem index is not in range: {0} - {SubItemsCount - 1}");

            return _m.MinSize + _m.IncrementStep * i;
        }

        /// <summary>
        /// Get total cost of I selection.
        /// </summary>
        /// <param name="i">Selection index between 0 and SubItemsCount - 1. 0 - means, that we select MinSize of items.</param>
        /// <returns>Total cost of selected merchendise items.</returns>
        public double GetNthPrice(int i)
        {
            return GetNthVolumeGallons(i) * _m.AvgPrice;
        }

        #region Comparing
        /// <summary>
        /// Compares two items against their cost. Is used for increasing sorting order (ASC).
        /// </summary>
        /// <param name="o1">Item first.</param>
        /// <param name="o2">Item second.</param>
        /// <returns>0 - if they are equal, -1 (or negative) when o1 < o2, 1 (or positive) when o1 > o2./returns>
        static int CostOrderCmp (MerchendiseBulkItem o1, MerchendiseBulkItem o2)
        {
            if (o1 == null)
                return o2 == null ? 0 : -1;
            else if (o2 == null)
                return 1;

            return o1._m.AvgPrice.CompareTo(o2._m.AvgPrice);
        }
        public static Comparison<MerchendiseBulkItem> CostOrder => CostOrderCmp;

        /// <summary>
        /// Compares two items against their cost. Is used for decreasing sorting order (DESC).
        /// </summary>
        /// <param name="o1">Item first.</param>
        /// <param name="o2">Item second.</param>
        /// <returns>0 - if they are equal, -1 (or negative) when o1 > o2, 1 (or positive) when o1 < o2./returns>
        static int CostOrderCmpDesc(MerchendiseBulkItem o1, MerchendiseBulkItem o2)
        {
            if (o2 == null)
                return o1 == null ? 0 : -1;
            else if (o1 == null)
                return 1;

            return o2._m.AvgPrice.CompareTo(o1._m.AvgPrice);
        }
        public static Comparison<MerchendiseBulkItem> CostOrderDesc => CostOrderCmpDesc;
        #endregion Comparing


        /// <summary>
        /// Returns the size of Cartesian product of all subitems (selection variants) of merchendise items.
        /// </summary>
        /// <param name="arr">Items</param>
        /// <returns>The size</returns>
        public static long CalcCartesianPower(MerchendiseBulkItem[] arr)
        {
            checked
            {
                return arr.Select(i => i.SubItemsCount + 1).Aggregate((acc, val) => acc * val);
            }
        }


        /// <summary>
        /// Return cartesian product of items specified in indexes. Returning is accomplished by means of sequential calling to the callback.
        /// </summary>
        /// <param name="arr">Merchecdise items.</param>
        /// <param name="indexes">Indexes of items to calculate the product.</param>
        /// <param name="callback">A callback to recieve the product.</param>
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

        /// <summary>
        /// Calculates Cartesian product of all the subitems. -1 means, that item is not selected.
        /// </summary>
        /// <param name="arr">Items.</param>
        /// <param name="callback">A callback to recieve the product.</param>
        public static void VariableCartesianIteration (MerchendiseBulkItem[] arr, Func<int[], bool> callback)
        {
            int n = arr.Length; //the number of arrays to product            
            int lastIdx = n - 1;

            int[] selectedAmt;
            if (n < 2)
            {
                selectedAmt = new int[1];
                var m = arr[0];
                int subItemsCnt = m.SubItemsCount;
                
                for (int i = 0; i < subItemsCnt; ++i)
                {
                    selectedAmt[0] = m.GetNthVolumeGallons(i);
                    if (!callback(selectedAmt))
                        break;
                }
                return;
            }

            int[] index = Enumerable.Repeat(-1, n).ToArray();
            index[ lastIdx ] = 0;
            selectedAmt = Enumerable.Repeat(-1, n).ToArray();

            while (index[0] < arr[0].SubItemsCount)
            {
                while (index[lastIdx] < arr[lastIdx].SubItemsCount)
                {
                    selectedAmt[ lastIdx ] = arr[ lastIdx ].GetNthVolumeGallons(index[ lastIdx ]);
                    if (!callback(selectedAmt))
                        return;
                    index[ lastIdx ]++;
                }

                index[lastIdx] = -1;
                int j = n - 2;
                while (j > 0)
                {
                    index[ j ]++;

                    if (index[j] < arr[j].SubItemsCount)
                    {
                        selectedAmt[ j ] = arr[ j ].GetNthVolumeGallons(index[ j ]);
                        break;
                    }

                    selectedAmt[ j ] = index[ j ] = -1;
                    j--;
                }

                if (j == 0)
                {
                    index[ 0 ]++;
                    if (index[0] < arr[0].SubItemsCount)
                        selectedAmt[ 0 ] = arr[ 0 ].GetNthVolumeGallons(index[ 0 ]);
                }
            }
        }
    }
}
