using System.Collections.Generic;
using System.Threading.Tasks;
using DataProviders.Contract;

namespace BackPackOptimizer.Contract
{
    /// <summary>
    /// Describes a generic optimizer for solving Backpack/Knpsack problem.
    /// </summary>
    public interface IBackpackOptimizer
    {
        /// <summary>
        /// Starting solving Backpack problem. Synchronous or asynchronous behaviour depends on the implementation.
        /// </summary>
        /// <param name="merchendises">The set of items, which could be put into the backpack.</param>
        /// <param name="requiredGallons">Backpack capacity (volume or weight).</param>
        /// <param name="solveMinimization">Indicates the type of task to solve: summary items cost minimization or maximization.</param>
        /// <returns></returns>
        Task<Purchases> Solve(IEnumerable<Merchendise> merchendises, int requiredGallons, bool solveMinimization);
    }
}
