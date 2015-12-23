using System;

namespace BackPackOptimizer.Contract
{
    /// <summary>
    /// Represents an end user recommendation regarding buying a particular merchendise item: what item and how many items to buy. 
    /// </summary>
    public struct Purchase: IComparable<Purchase>, IComparable, IEquatable<Purchase>
    {
        /// <summary>
        /// Item name.
        /// </summary>
        public string SourceName { get; set; }
        /// <summary>
        /// Items count.
        /// </summary>
        public int NumberOfGallons { get; set; }
        /// <summary>
        /// Cost of one item.
        /// </summary>
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
            return 381 ^ (SourceName ?? string.Empty).GetHashCode();
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
            

            return string.Compare(SourceName, other.SourceName, StringComparison.Ordinal);
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
            return string.Equals(SourceName, val.SourceName, StringComparison.Ordinal);
        }
        #endregion Equatable<T>  
    }
}
