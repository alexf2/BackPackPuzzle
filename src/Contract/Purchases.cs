using System;
using System.Linq;
using Wintellect.PowerCollections;

namespace BackPackOptimizer.Contract
{
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
        public int TotalGallons => _totalGallons ?? (_totalGallons = Merchendises.Sum(pc => pc.NumberOfGallons)).Value;

        public OrderedBag<Purchase> Merchendises { get; private set; }

        double? _averagePriceOfGallon;
        public double AveragePriceOfGallon => _averagePriceOfGallon ?? (_averagePriceOfGallon = Merchendises.Average(pc => pc.PriceOfGallon)).Value;

        public double TotalCost => Merchendises.Count == 0 ? 0:AveragePriceOfGallon * TotalGallons;
    }
}
