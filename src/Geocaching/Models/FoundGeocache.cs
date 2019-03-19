using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocaching.Models
{
    public class FoundGeocache
    {
        public int PersonID { get; set; }
        public Person Person { get; set; }

        public int GeocacheID { get; set; }
        public Geocache Geocache { get; set; }

    }
}
