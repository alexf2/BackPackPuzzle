using BackPackOptimizer.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProviders.Contract;

namespace BackPackOptimizer.Clients.Console
{
    public sealed class BpOptimizerApp
    {
        readonly IBackpackOptimizer _optimizer;
        readonly IMerchendiseProvider _provider;
        readonly int _backpackSize;

        public BpOptimizerApp(IBackpackOptimizer optimizer, IMerchendiseProvider provider, int backpackSize)
        {
            _optimizer = optimizer;
            _provider = provider;
            _backpackSize = backpackSize;
        }

        public async Task<Purchases> Run()
        {
            return await _optimizer.Solve(_provider.Merchendises, _backpackSize);
        }
    }
}
