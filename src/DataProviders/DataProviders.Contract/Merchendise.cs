using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace DataProviders.Contract
{
    public struct Merchendise
    {
        [Required]
        public string Name { get; set; }

        [Min(1)]
        public int Size { get; set;  }

        [Min(0.01)]
        public decimal AvgPrice { get; set; }

        [Min(1)]
        public int MinSize { get; set; }

        [Min(1)]
        public int IncrementStep { get; set; }
    }
}
