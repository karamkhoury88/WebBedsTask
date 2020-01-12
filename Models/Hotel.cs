using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBedsTask.Models
{
    public class Hotel
    {
        public long PropertyID { get; set; }
        public string Name { get; set; }
        public long GeoId { get; set; }
        public int Rating { get; set; }
    }
}
