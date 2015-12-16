using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProviders.Contract
{
    public interface IMerchendiseProvider
    {
        IEnumerable<Merchendise> Merchendises { get; }
    }
}
