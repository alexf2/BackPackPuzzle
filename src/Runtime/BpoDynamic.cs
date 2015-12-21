﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackPackOptimizer.Contract;
using DataProviders.Contract;

namespace BackPackOptimizer.Runtime
{
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

            //return await Task.FromResult(CreateFullPurchasing(merchendises));
            

            MerchendiseBulkItem[] items = MerchendiseBulkItem.ToBulkItems(merchendises);
            //NegateCost(items);

            var instantPurchases = TryInstantSolution(items, requiredGallons);
            if (instantPurchases != null) //the task is either: has an obvious solution or doesn't have any solution
                return Task.FromResult(instantPurchases);

            

            long totalIterations = ((long)requiredGallons + 1L) * (long)items.Length;
            long notifyStep = CalculateNotifyStep(totalIterations);

            return Task<Purchases>.Factory.StartNew(() =>
            {                
                Purchases[] ic = new Purchases[requiredGallons + 1];
                for (int i = 0; i < ic.Length; i++)
                    ic[ i ] = new Purchases();

                long iterCount = 0;
                for (int i = 0; i < items.Length; i++)
                    for (int j = requiredGallons; j >= 0; j--) //capacity
                    {
                        if (++iterCount % notifyStep == 0)
                            NotifyProgress(iterCount, totalIterations);

                        _cancelToken.ThrowIfCancellationRequested();

                        var item = items[ i ];

                        if (j >= item.Merchendise.MinSize)
                        {
                            int quantity = Math.Min(item.Merchendise.Size, j / item.Merchendise.MinSize);
                            for (int k = item.Merchendise.MinSize; k <= quantity; k += item.Merchendise.IncrementStep)
                            {
                                Purchases pchLighter = ic[ j - k ];
                                double testValue = pchLighter.TotalCost + k * item.Merchendise.AvgPrice;
                                if (testValue > ic[j].TotalCost)
                                    (ic[ j ] = (Purchases)pchLighter.Clone()).Add(new Purchase() { SourceName = item.Merchendise.Name, NumberOfGallons = k, PriceOfGallon = item.Merchendise.AvgPrice}); 
                            }
                        }
                    }

                FinalNotify();
                //NegateCost(items);
                return ic[ requiredGallons ];

            }, _cancelToken);
        }

        void NegateCost (IEnumerable<MerchendiseBulkItem> items)
        {
            //foreach (var item in items)
                //item.Merchendise.AvgPrice = -item.Merchendise.AvgPrice;

            double avg = (items.Min(i => i.Merchendise.AvgPrice) + items.Max(i => i.Merchendise.AvgPrice)) / 2.0;
            
            foreach (var item in items)
                if (item.Merchendise.AvgPrice > avg)
                    item.Merchendise.AvgPrice = avg - 2*(item.Merchendise.AvgPrice - avg);
                else if (item.Merchendise.AvgPrice < avg)
                    item.Merchendise.AvgPrice = avg + 2*(avg - item.Merchendise.AvgPrice);

            //foreach (var item in items)
                //Console.WriteLine($"{item.Merchendise.Name}: {item.Merchendise.AvgPrice}");
        }
    }
}
