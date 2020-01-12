using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBedsTask.Models
{
    public class Rate
    {
        /// <summary>
        /// It has 2 types:
        /// 1:PerNight
        /// 2:Stay
        /// </summary>
        public string RateType { get; set; }

        /// <summary>
        /// It has 3 types:
        /// 1:No Meal
        /// 2:Half Board
        /// 3:Full Board
        /// </summary>
        public string BoardType { get; set; }

        public double Value { get; set; }

        public double TotalPrice { get; set; }

    }
}
