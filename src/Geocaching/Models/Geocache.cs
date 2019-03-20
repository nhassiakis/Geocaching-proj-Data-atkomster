using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocaching.Models
{
    public class Geocache
    {
        [Key]
        public int ID { get; set; }
        public Person Person { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        [MaxLength(255)]
        public string Contents { get; set; }
        [MaxLength(255)]
        public string Message { get; set; }
        public IList<FoundGeocache> FoundGeocaches { get; set; }

    }
}
