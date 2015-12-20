using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackPackOptimizer.Contract;
using DataProviders.Contract;
using Wintellect.PowerCollections;

namespace BackPackOptimizer.Runtime
{
    public class BpoBruteForce: BpoBase, IBackpackOptimizer
    {        
        public BpoBruteForce(IExecutionContext context) : base(context)
        {            
        }

        public Task<Purchases> Solve(IEnumerable<Merchendise> merchendises, int requiredGallons)
        {
            if (merchendises == null)
                throw new ArgumentNullException(nameof(merchendises));
            if (requiredGallons < 1)
                throw new ArgumentOutOfRangeException($"{nameof(requiredGallons)} should be greater then zero");


            //return await Task.FromResult(CreateFullPurchasing(merchendises));
            //return await CreateTestTask(merchendises);

            MerchendiseBulkItem[] items = MerchendiseBulkItem.ToBulkItems(merchendises);

            return Task<Purchases>.Factory.StartNew(() =>
            {
                var mainIter = new BinomiaUnorderedFulllIterator<MerchendiseBulkItem>(items);
                long totalIterations;
                try
                {
                    totalIterations = mainIter.TotalCombinationsCount;
                }
                catch (OverflowException ex)
                {
                    throw new Exception("The task is really enormouos to solve", ex);
                }
                long notifyStep = CalculateNotifyStep(totalIterations);

                int[] solutionItemIndexes = null, solutionItemSubIndexes = null;
                double solutionCost = double.MaxValue;
                int solutionGallons = 0;

                long iterCount = 0;
                foreach (var combinationIdx in mainIter.Iterate())
                {
                    iterCount++;
                    if (iterCount % notifyStep == 0)
                        NotifyProgress(iterCount, totalIterations);

                    _cancelToken.ThrowIfCancellationRequested();                    

                    MerchendiseBulkItem.CartesianProductIteration(items, combinationIdx, (cartesianSelection) =>
                    {
                        double cost;
                        int amountGallons;
                        CalculateCombinationCost(items, combinationIdx, cartesianSelection, out cost, out amountGallons);

                        if (amountGallons >= requiredGallons && cost < solutionCost)
                        {
                            solutionItemIndexes = (int[])combinationIdx.Clone();
                            solutionItemSubIndexes = (int[])cartesianSelection.Clone();
                            solutionCost = cost;
                            solutionGallons = amountGallons;
                        }

                        return !_cancelToken.IsCancellationRequested;
                    });
                }

                var resSet = new OrderedBag<Purchase>();
                if (solutionItemIndexes != null)
                    FillPurchasesSet(resSet, items, solutionItemIndexes, solutionItemSubIndexes);

                return new Purchases(resSet);
            }, _cancelToken);            
        }

        static void CalculateCombinationCost(MerchendiseBulkItem[] data, int[] indexes, int[] subIndexes, out double cost, out int gallons)
        {
            cost = 0;
            gallons = 0;
            for (int i = 0; i < indexes.Length; i++)
            {
                var merchendise = data[ indexes[i] ];
                cost += merchendise.GetNthPrice(subIndexes[ i ]);
                gallons += merchendise.GetNthVolumeGallons(subIndexes[i]);
            }
        }

        static void FillPurchasesSet(OrderedBag<Purchase> bag, MerchendiseBulkItem[] data, int[] indexes, int[] subIndexes)
        {
            for (int i = 0; i < indexes.Length; i++)
                bag.Add(new Purchase(data[indexes[i]], subIndexes[i]));
        }
    }
}
