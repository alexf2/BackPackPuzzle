using System;
using System.Threading;

namespace BackPackOptimizer.Contract
{
    public interface IExecutionContext
    {
        IProgress<ProgressInfo> ProgressCallback { get; }
        CancellationTokenSource CancelSource { get; }
    }
}
