using System;
using System.Linq;
using Wintellect.PowerCollections;

namespace BackPackOptimizer.Contract
{
    /// <summary>
    /// Represents a complete set of purchases.
    /// </summary>
    public sealed class Purchases: ICloneable
    {
        public Purchases()
        {
            Merchendises = new OrderedBag<Purchase>();
        }

        public Purchases(OrderedBag<Purchase> merchendises)
        {
            if (merchendises == null)
                throw new ArgumentNullException(nameof(merchendises));

            Merchendises = merchendises;
        }

        public void Add(Purchase p)
        {
            if (p == null)
                throw new ArgumentNullException(nameof(p));

            if (Merchendises.Contains(p))
            {
                Purchase tmp = Merchendises[ Merchendises.IndexOf(p) ];
                Merchendises.Remove(p);
                tmp.NumberOfGallons += p.NumberOfGallons;
                Merchendises.Add(tmp);
            }
            else
                Merchendises.Add(p);

            if (_totalGallons.HasValue)
                _totalGallons += p.NumberOfGallons;
            _averagePriceOfGallon = null;
        }

        public object Clone()
        {
            return new Purchases(new OrderedBag<Purchase>(Merchendises))
            {
                _totalGallons = _totalGallons,
                _averagePriceOfGallon = _averagePriceOfGallon
            };
        }

        int? _totalGallons;
        /// <summary>
        /// Return total items number.
        /// </summary>
        public int TotalGallons => _totalGallons ?? (_totalGallons = Merchendises.Sum(pc => pc.NumberOfGallons)).Value;

        /// <summary>
        /// Returns the detailed list of ourchases.
        /// </summary>
        public OrderedBag<Purchase> Merchendises { get; private set; }

        double? _averagePriceOfGallon;
        /// <summary>
        /// Returns cost of one item.
        /// </summary>
        public double AveragePriceOfGallon => _averagePriceOfGallon ?? (_averagePriceOfGallon = Merchendises.Average(pc => pc.PriceOfGallon)).Value;

        /// <summary>
        /// Returns overall purchase cost.
        /// </summary>
        public double TotalCost => Merchendises.Count == 0 ? 0:AveragePriceOfGallon * TotalGallons;
    }
}
