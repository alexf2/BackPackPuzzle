using System;
using System.Collections.Generic;
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

        public async Task<Purchases> Solve(IEnumerable<Merchendise> merchendises, int backpackSize)
        {
            if (merchendises == null)
                throw new ArgumentNullException(nameof(merchendises));
            if (backpackSize < 1)
                throw new ArgumentOutOfRangeException($"{nameof(backpackSize)} should be greater then zero");

            return await Task.FromResult(CreateFullPurchasing(merchendises));
        }
    }
}
