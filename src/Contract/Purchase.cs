using System;

namespace BackPackOptimizer.Contract
{
    public struct Purchase: IComparable<Purchase>, IComparable, IEquatable<Purchase>
    {
        public string SourceName { get; set; }
        public int NumberOfGallons { get; set; }
        public double PriceOfGallon { get; set; }
        
        public Purchase(MerchendiseBulkItem bulkItem, int subIndex)
        {
            SourceName = bulkItem.Merchendise.Name;
            NumberOfGallons = bulkItem.GetNthVolumeGallons(subIndex);
            PriceOfGallon = bulkItem.Merchendise.AvgPrice;
        }

        #region Object
        public override int GetHashCode()
        {
            return 381 ^ (SourceName ?? string.Empty).GetHashCode() ^ NumberOfGallons;
        }

        public override bool Equals(object obj)
        {
            return obj is Purchase && Equals((Purchase)obj);
        }

        public override string ToString() => $"{NumberOfGallons} gallons from '{SourceName}', with price {PriceOfGallon.ToString("#.##")}";
        #endregion Object

        #region Operators
        public static bool operator ==(Purchase p1, Purchase p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(Purchase p1, Purchase p2)
        {
            return !p1.Equals(p1);
        }

        public static bool operator < (Purchase p1, Purchase p2)
        {
            return p1.CompareTo(p2) < 0;
        }
        public static bool operator > (Purchase p1, Purchase p2)
        {
            return p1.CompareTo(p2) > 0;
        }
        #endregion Operators

        #region IComparable<T>
        public int CompareTo(Purchase other)
        {
            if (Equals(other))
                return 0;

            if (SourceName == null && other.SourceName == null)
                return 0; // same
            if (SourceName == null ^ other.SourceName == null)
                return SourceName == null ? 1 : -1;
            

            return NumberOfGallons.CompareTo(other.NumberOfGallons);
        }
        #endregion IComparable<T>

        #region IComparable
        int IComparable.CompareTo(object other)
        {
            if (!(other is Purchase))
                throw new InvalidOperationException("CompareTo: Not a Purchase");

            return CompareTo((Purchase)other);
        }
        #endregion IComparabl

        #region Equatable<T>
        public bool Equals(Purchase val)
        {
            return SourceName == val.SourceName && NumberOfGallons == val.NumberOfGallons;
        }
        #endregion Equatable<T>  
    }
}
