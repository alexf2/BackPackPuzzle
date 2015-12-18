using System.Collections.Generic;

namespace DataProviders.Contract
{
    public interface IMerchendiseProvider
    {
        IEnumerable<Merchendise> Merchendises { get; }
    }
}
