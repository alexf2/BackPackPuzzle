using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackPackOptimizer.Contract;
using DataProviders.Contract;
using Wintellect.PowerCollections;

namespace BackPackOptimizer.Runtime
{
    public sealed class BpoGreedy : BpoBase, IBackpackOptimizer
    {
        public BpoGreedy(IExecutionContext context) : base(context)
        {
        }

        public Task<Purchases> Solve(IEnumerable<Merchendise> merchendises, int requiredGallons, bool solveMinimization)
        {
            if (merchendises == null)
                throw new ArgumentNullException(nameof(merchendises));
            if (requiredGallons < 1)
                throw new ArgumentOutOfRangeException($"{nameof(requiredGallons)} should be greater then zero");            

            //return await Task.FromResult(CreateFullPurchasing(merchendises));

            MerchendiseBulkItem[] items = MerchendiseBulkItem.ToBulkItems(merchendises);
            var instantPurchases = TryInstantSolution(items, requiredGallons);
            if (instantPurchases != null) //the task is either: has an obvious solution or doesn't have any solution
                return Task.FromResult(instantPurchases);
            

            long notifyStep = CalculateNotifyStep(items.Length);
            NotifyProgress("Sorting...");
            Array.Sort(items, solveMinimization ? MerchendiseBulkItem.CostOrder: MerchendiseBulkItem.CostOrderDesc);

            return Task<Purchases>.Factory.StartNew(() =>
            {
                int currentGallons = requiredGallons;
                var resSet = new OrderedBag<Purchase>();

                NotifyProgress("Greedy collecting items...");
                for (int i = 0; i < items.Length && currentGallons > 0; ++i)
                {
                    if ((i + 1) % notifyStep == 0)
                        NotifyProgress(i + 1, items.Length);

                    var item = items[i];
                    int size;
                    if (item.Merchendise.Size <= currentGallons)
                        size = item.Merchendise.Size;
                    else if (requiredGallons >= item.Merchendise.MinSize)
                    {
                        size = item.Merchendise.MinSize;
                        while (size < currentGallons && size < item.Merchendise.Size)
                            size += item.Merchendise.IncrementStep;                        
                    }
                    else
                        continue;

                    currentGallons -= size;
                    resSet.Add(new Purchase() { SourceName = item.Merchendise.Name, NumberOfGallons = size, PriceOfGallon = item.Merchendise .AvgPrice});
                }
                
                if (resSet.Sum(item => item.NumberOfGallons) < requiredGallons)
                    resSet.Clear();

                FinalNotify();
                return new Purchases(resSet);

            }, _cancelToken);
        }
    }
}
