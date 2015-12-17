using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackPackOptimizer.Contract;
using DataProviders.Contract;

namespace BackPackOptimizer.Runtime
{
    public class BpoBase
    {        
        protected readonly IProgress<ProgressInfo> _progress;
        protected readonly CancellationToken _cancelToken;

        public BpoBase(IExecutionContext context)
        {            
            _progress = context.ProgressCallback;
            _cancelToken = context.Token;
        }
    }
}
