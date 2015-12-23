using System.Linq;
using BackPackOptimizer.Contract;
using System.Threading.Tasks;
using DataProviders.Contract;

namespace BackPackOptimizer.Clients.Console
{
    /// <summary>
    /// Represents optimizator application.
    /// </summary>
    public sealed class BpOptimizerApp
    {
        readonly IBackpackOptimizer _optimizer;
        readonly IMerchendiseProvider _provider;
        readonly int _backpackSize;
        readonly bool _solveMinimization;

        public BpOptimizerApp(IBackpackOptimizer optimizer, IMerchendiseProvider provider, int backpackSize, bool solveMinimization)
        {
            _optimizer = optimizer;
            _provider = provider;
            _backpackSize = backpackSize;
            _solveMinimization = solveMinimization;
        }

        public async Task<Purchases> Run()
        {
            return await _optimizer.Solve(_provider.Merchendise, _backpackSize, _solveMinimization);
        }

        public int TotalGallons => _provider.Merchendise.Sum((m) => m.Size);
    }
}
