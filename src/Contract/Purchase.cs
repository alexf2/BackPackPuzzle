using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackPackOptimizer.Contract
{
    public struct Purchase
    {
        public string SourceNamr { get; set; }
        public short NumberOfGallons { get; set; }
        public float PriceOfGallon { get; set; }
    }
}
