using System;
using System.Diagnostics;
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
        public int NumberOfGallons
        {
            get
            {
                if (_numberOfGallons == null)
                    _numberOfGallons = Merchendises.Sum(pc => pc.NumberOfGallons);

                return _numberOfGallons.Value;
            }            
        }

        public OrderedBag<Purchase> Merchendises { get; private set; }

        float? _averagePriceOfGallon;
        public float AveragePriceOfGallon
        {
            get
            {
                if (_averagePriceOfGallon == null)
                    _averagePriceOfGallon = (float)Merchendises.Average(pc => pc.PriceOfGallon);

                return _averagePriceOfGallon.Value;
            }
        }
    }
}
