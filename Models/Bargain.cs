using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBedsTask.Models
{
    /// <summary>
    /// This class represent the Supplier API response of findBargain method
    /// </summary>
    public class Bargain
    {
        public Hotel Hotel { get; set; }
        public List<Rate> Rates { get; set; }
    }
}
