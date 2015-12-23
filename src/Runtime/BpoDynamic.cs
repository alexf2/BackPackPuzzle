using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackPackOptimizer.Contract;
using DataProviders.Contract;
using Wintellect.PowerCollections;

namespace BackPackOptimizer.Runtime
{
    /// <summary>
    /// Implements Dynamic Programming algorithm to solve Backpack optimization task.
    /// Takes O(N * M).
    /// </summary>
    public sealed class BpoDynamic : BpoBase, IBackpackOptimizer
    {
        public BpoDynamic(IExecutionContext context) : base(context)
        {
        }

        public Task<Purchases> Solve(IEnumerable<Merchendise> merchendises, int requiredGallons, bool solveMinimization)
        {
            if (merchendises == null)
                throw new ArgumentNullException(nameof(merchendises));
            if (requiredGallons < 1)
                throw new ArgumentOutOfRangeException($"{nameof(requiredGallons)} should be greater then zero");

                        

            MerchendiseBulkItem[] itemsOrig = MerchendiseBulkItem.ToBulkItems(merchendises);
            MerchendiseBulkItem[] items = MerchendiseBulkItem.ToBulkItems(merchendises);
            if (solveMinimization)
                NegateCost(items);
            NormalizeCosts(items);

            var instantPurchases = TryInstantSolution(items, requiredGallons);
            if (instantPurchases != null) //the task is either: has an obvious solution or doesn't have any solution
                return Task.FromResult(instantPurchases);
                        

            return Task<Purchases>.Factory.StartNew(() =>
            {
                int N = items.Length; //spicies number

                //progress parameters
                long totalIterations = (long)(requiredGallons + 1) * (long)N;
                long notifyStep = CalculateNotifyStep(totalIterations);
                long iterCount = 0;

                int[] opts = new int[N + 1];
                int[] P = new int[N + 1]; P[ 0 ] = 1; //item encoding
                int choose = 0;

                for (int j = 0; j < N; j++)
                {
                    var merch = items[ j ];

                    opts[j + 1] = opts[j] + merch.SubItemsCount;
                    P[j + 1] = P[ j ]*(1 + merch.SubItemsCount);
                }

                int[,] m = (int[,])Array.CreateInstance(typeof(int), new[] { requiredGallons + 1, opts[N] + 1 }); //maximum pack
                int[,] b = (int[,])Array.CreateInstance(typeof(int), new[] { requiredGallons + 1, opts[N] + 1 }); //best selection 

                for (int w = 1; w <= requiredGallons; w++)
                    for (int j = 0; j < N; j++)
                    {
                        if (++iterCount % notifyStep == 0)
                            NotifyProgress(iterCount, totalIterations);

                        _cancelToken.ThrowIfCancellationRequested();

                        var merch = items[ j ];
                        int @base = opts[ j ]; //item index for 0

                        for (int n = 1; n <= merch.SubItemsCount; n++)
                        {
                            int W = merch.GetNthVolumeGallons(n - 1), //calculate selected weight (gallons amount)
                                s = w >= W ? 1 : 0, //checking whether it fits into current backpack size
                                v = s * GetNormalizedCost(merch, n - 1), //calculating the cost for selected amount
                                I = @base + n, //item number for the selection
                                wN = w - s*W, //calculating how much other species we could add
                                C = n*P[j] + b[wN, @base]; //encoded combination

                            m[w, I] = Math.Max(m[w, I - 1], v + m[wN, @base]); //most optimal value
                            choose = b[w, I] = (m[w, I] > m[w, I - 1] ? C : b[w, I - 1]);
                        }
                    }

                int[] best = new int[ N ];
                for (int j = N - 1; j >= 0; j--) //calculating selection indexes (for each merchendise whether it is taken (< 0) and how many items/gallons)
                {
                    best[ j ] = (int)Math.Floor((double)choose / P[j]);
                    choose -= best[j] * P[j];
                }

                var resSet = new OrderedBag<Purchase>();
                if (best.Any( i => i != 0))
                    for (int i = 0; i < N; i++)
                    {
                        if (best[i] == 0)
                            continue;

                        var merch = itemsOrig[ i ].Merchendise;
                        resSet.Add(new Purchase() {SourceName = merch.Name, NumberOfGallons = itemsOrig[ i ].GetNthVolumeGallons(best[i] - 1), PriceOfGallon = merch.AvgPrice});
                    }

                FinalNotify();                
                
                return new Purchases(resSet);
            }, _cancelToken);
        }

        static int GetNormalizedCost(MerchendiseBulkItem m, int number)
        {
            return (int) (m.GetNthPrice(number)*100.0);
        }

        static void NegateCost (IEnumerable<MerchendiseBulkItem> items)
        {            
            double avg = (items.Min(i => i.Merchendise.AvgPrice) + items.Max(i => i.Merchendise.AvgPrice)) / 2.0;
            
            foreach (var item in items)
                if (item.Merchendise.AvgPrice > avg)
                    item.Merchendise.AvgPrice = avg - 2*(item.Merchendise.AvgPrice - avg);
                else if (item.Merchendise.AvgPrice < avg)
                    item.Merchendise.AvgPrice = avg + 2*(avg - item.Merchendise.AvgPrice);

            //foreach (var item in items)
                //Console.WriteLine($"{item.Merchendise.Name}: {item.Merchendise.AvgPrice}");
        }

        static void NormalizeCosts (IList<MerchendiseBulkItem> items)
        {
            long cf = CommonFactor.Calculate(items, (item) => (long) (item.Merchendise.AvgPrice*100.0), items.Count);
            foreach (var item in items)            
                item.Merchendise.AvgPrice = (long)(item.Merchendise.AvgPrice*100.0) / cf;
        }
    }
}
