using Wintellect.PowerCollections;

namespace BackPackOptimizer.Contract
{
    public sealed class Purchases
    {
        public Purchases(int ng, OrderedBag<Purchase> pc, float avgP)
        {
            NumberOfGallons = ng;
            Merchendises = pc;
            AveragePriceOfGallon = avgP;
        }

        public int NumberOfGallons { get; private set; }
        public OrderedBag<Purchase> Merchendises { get; private set; }
        public float AveragePriceOfGallon { get; private set; }
    }
}
