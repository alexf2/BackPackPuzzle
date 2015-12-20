using System;
using System.Linq;
using Wintellect.PowerCollections;

namespace BackPackOptimizer.Contract
{
    public sealed class Purchases
    {
        public Purchases(OrderedBag<Purchase> merchendises)
        {
            if (merchendises == null)
                throw new ArgumentNullException(nameof(merchendises));

            Merchendises = merchendises;            
        }

        int? _numberOfGallons;
        public int NumberOfGallons => _numberOfGallons ?? (_numberOfGallons = Merchendises.Sum(pc => pc.NumberOfGallons)).Value;

        public OrderedBag<Purchase> Merchendises { get; private set; }

        double? _averagePriceOfGallon;
        public double AveragePriceOfGallon => _averagePriceOfGallon ?? (_averagePriceOfGallon = Merchendises.Average(pc => pc.PriceOfGallon)).Value;
    }
}
