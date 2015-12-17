using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackPackOptimizer.Contract;
using DataProviders.Contract;

namespace BackPackOptimizer.Runtime
{
    public sealed class BpoDynamic : IBackpackOptimizer
    {
        public Purchases Solve(IEnumerable<Merchendise> merchendises, int backpackSize)
        {
            return null;
        }
    }
}
