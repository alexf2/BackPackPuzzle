using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataProviders.Contract;

namespace BackPackOptimizer.Contract
{
    public interface IBackpackOptimizer
    {
        Task<Purchases> Solve(IEnumerable<Merchendise> merchendises, int requiredGallons);
    }
}
