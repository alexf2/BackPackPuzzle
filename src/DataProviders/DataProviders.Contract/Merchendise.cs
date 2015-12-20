using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace DataProviders.Contract
{
    /// <summary>
    /// Describes a bounded set of items.
    /// </summary>
    public class Merchendise
    {
        /// <summary>
        /// Item name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Items total amount in pcs. 
        /// </summary>
        [Min(1)]
        public int Size { get; set;  }

        /// <summary>
        /// Average cost of an item. 
        /// </summary>
        [Min(0.01)]
        public double AvgPrice { get; set; }

        /// <summary>
        /// Minimum items count to take at once. I.e. we can not take less items.
        /// </summary>
        [Min(1)]
        public int MinSize { get; set; }

        /// <summary>
        /// The bulk size. I.e. how many items might be added to increase taken amount until Size have reached.
        /// </summary>
        [Min(1)]
        public int IncrementStep { get; set; }        
    }
}
