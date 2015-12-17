using System.Collections.Generic;
using DataProviders.Contract;

namespace BackPackOptimizer.Contract
{
    public interface IBackpackOptimizer
    {
        Purchases Solve(IEnumerable<Merchendise> merchendises, int backpackSize);
    }
}
