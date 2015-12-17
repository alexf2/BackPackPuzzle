using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BackPackOptimizer.Contract;
using DataProviders.Contract;
using Wintellect.PowerCollections;

namespace BackPackOptimizer.Runtime
{
    public class BpoBase
    {        
        protected readonly IProgress<ProgressInfo> _progress;
        protected readonly CancellationToken _cancelToken;

        protected BpoBase(IExecutionContext context)
        {            
            _progress = context.ProgressCallback;
            _cancelToken = context.CancelSource.Token;
        }

        protected static Purchases CreateFullPurchase(IEnumerable<Merchendise> merchendises)
        {
            var bag = new OrderedBag<Purchase>(merchendises.Select(m => new Purchase() { SourceName = m.Name, NumberOfGallons = m.Size, PriceOfGallon = (float)m.AvgPrice}));
            return new Purchases(bag);
        }
    }
}
