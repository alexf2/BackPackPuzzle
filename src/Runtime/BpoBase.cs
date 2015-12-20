using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackPackOptimizer.Contract;
using DataProviders.Contract;
using Wintellect.PowerCollections;

namespace BackPackOptimizer.Runtime
{
    public class BpoBase
    {
        const int NotifyStepPercent = 10;

        protected readonly IProgress<ProgressInfo> _progress;
        protected readonly CancellationToken _cancelToken;

        protected BpoBase(IExecutionContext context)
        {            
            _progress = context.ProgressCallback;
            _cancelToken = context.CancelSource.Token;
        }

        protected static Purchases CreateFullPurchasing(IEnumerable<Merchendise> merchendises)
        {
            var bag = new OrderedBag<Purchase>(merchendises.Select(m => new Purchase() { SourceName = m.Name, NumberOfGallons = m.Size, PriceOfGallon = (float)m.AvgPrice}));
            return new Purchases(bag);
        }

        protected Purchases TryInstantSolution(IEnumerable<MerchendiseBulkItem> items, int requiredGallons)
        {
            int totalGallons = items.Sum(i => i.Merchendise.Size);

            if (totalGallons < requiredGallons)
                return new Purchases(new OrderedBag<Purchase>()); //no solution

            if (totalGallons == requiredGallons)
            {
                var bag = new OrderedBag<Purchase>();
                foreach(var item in items)
                    bag.Add(new Purchase(item, item.SubItemsCount - 1));

                return new Purchases(bag);
            }

            return null;
        }

        protected Task<Purchases> CreateTestTask(IEnumerable<Merchendise> merchendises)
        {
            var m = merchendises.ToArray();            
            int step = (int)CalculateNotifyStep(m.Length);

            var task = Task<Purchases>.Factory.StartNew(() =>
            {
                for (int i = 0; i < m.Length; i += step)
                {
                    NotifyProgress(i + 1, m.Length);
                    Task.Delay(500, _cancelToken).Wait(_cancelToken);
                    if (_cancelToken.IsCancellationRequested)
                        return null;
                }
                return CreateFullPurchasing(m);
            }, _cancelToken);
            return task;
        }

        protected void NotifyProgress(long iteration, long totalIterations)
        {
            _progress?.Report(new ProgressInfo {Iteration = iteration, TotalIterations = totalIterations});
        }

        protected void NotifyProgress(string msg)
        {
            _progress?.Report(new ProgressInfo { CustomMessage = msg});
        }

        protected static long CalculateNotifyStep(long totalTaskSize)
        {
            return (long)Math.Ceiling(Math.Max(totalTaskSize, 1L) / 100M * 10M);
        }
    }
}
