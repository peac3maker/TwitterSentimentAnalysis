using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterSearchGui
{
    public class BitCoinData
    {
        public DateTime Date { get; set; }
        public float Open { get; set; }
        public float Close { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public double VolumeBTC { get; set; }
        public double VolumeCurrency { get; set; }
        public float WeighedPrice { get; set; }
    }
}
