using System.Collections.Generic;

namespace DataProviders.Contract
{
    /// <summary>
    /// A generic provider of merchendises. Aimed to decouple consumers from a data source type. Wraps any data source whatever we may use (CSV, TXT, XML, etc).
    /// </summary>
    public interface IMerchendiseProvider
    {
        /// <summary>
        /// Returns a collection of merchendise, loaded from underlying storage.
        /// </summary>
        IEnumerable<Merchendise> Merchendise { get; }
    }
}
