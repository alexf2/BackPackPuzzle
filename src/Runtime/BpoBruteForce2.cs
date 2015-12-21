using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackPackOptimizer.Contract;
using DataProviders.Contract;
using Wintellect.PowerCollections;

namespace BackPackOptimizer.Runtime
{
    public class BpoBruteForce2 : BpoBase, IBackpackOptimizer
    {
        public BpoBruteForce2(IExecutionContext context) : base(context)
        {
        }

        public Task<Purchases> Solve(IEnumerable<Merchendise> merchendises, int requiredGallons, bool solveMinimization)
        {
            if (merchendises == null)
                throw new ArgumentNullException(nameof(merchendises));
            if (requiredGallons < 1)
                throw new ArgumentOutOfRangeException($"{nameof(requiredGallons)} should be greater then zero");
            

            MerchendiseBulkItem[] items = MerchendiseBulkItem.ToBulkItems(merchendises);
            var instantPurchases = TryInstantSolution(items, requiredGallons);
            if (instantPurchases != null) //the task is either: has an obvious solution or doesn't have any solution
                return Task.FromResult(instantPurchases);

            return Task<Purchases>.Factory.StartNew(() =>
            {                
                long totalIterations;
                try
                {
                    totalIterations = MerchendiseBulkItem.CalcCartesianPower(items);
                }
                catch (OverflowException ex)
                {
                    throw new Exception("The task is really enormouos to solve", ex);
                }
                long notifyStep = CalculateNotifyStep(totalIterations);

                int[] solutionVariableValues = null;
                double solutionCost = solveMinimization ? double.MaxValue : double.MinValue;
                int solutionGallons = 0;

                long iterCount = 0;
                MerchendiseBulkItem.VariableCartesianIteration(items, (variableValues) =>
                {
                    if (++iterCount % notifyStep == 0)
                        NotifyProgress(iterCount, totalIterations);

                    _cancelToken.ThrowIfCancellationRequested();

                    double cost;
                    int amountGallons;
                    CalculateCombinationCost(items, variableValues, out cost, out amountGallons);

                    if (solveMinimization ? (amountGallons >= requiredGallons && cost < solutionCost) : (amountGallons <= requiredGallons && cost > solutionCost))
                    {
                        solutionVariableValues = (int[])variableValues.Clone();
                        solutionCost = cost;
                        solutionGallons = amountGallons;
                    }

                    return !_cancelToken.IsCancellationRequested;
                });
                

                var resSet = new OrderedBag<Purchase>();
                if (solutionVariableValues != null)
                    FillPurchasesSet(resSet, items, solutionVariableValues);

                FinalNotify();
                return new Purchases(resSet);

            }, _cancelToken);
        }

        static void CalculateCombinationCost(MerchendiseBulkItem[] data, int[] varVlaues, out double cost, out int gallons)
        {
            cost = 0;
            gallons = 0;
            for (int i = 0; i < varVlaues.Length; i++)
            {
                if (varVlaues[i] < 0)
                    continue;
                var merchendise = data[ i ];
                cost += merchendise.Merchendise.AvgPrice*(double) varVlaues[ i ];
                gallons += varVlaues[ i ];
            }
        }

        static void FillPurchasesSet(OrderedBag<Purchase> bag, MerchendiseBulkItem[] data, int[] varValues)
        {
            for (int i = 0; i < varValues.Length; i++)
                if (varValues[i] > -1)
                    bag.Add(new Purchase() {SourceName = data[i].Merchendise.Name, NumberOfGallons = varValues[i], PriceOfGallon = data[i].Merchendise.AvgPrice});
        }
    }
}
