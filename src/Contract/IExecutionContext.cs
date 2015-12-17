using System;
using System.Threading;

namespace BackPackOptimizer.Contract
{
    public interface IExecutionContext
    {
        IProgress<ProgressInfo> ProgressCallback { get; }
        CancellationToken Token { get; }
    }
}
