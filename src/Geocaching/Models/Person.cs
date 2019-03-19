using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Geocaching.Models
{
    public class Person
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        [MaxLength(50)]
        public string Country { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(50)]
        public string StreetName { get; set; }
        [MaxLength(50)]
        public Int16 StreetNumber { get; set; }
        public IList<FoundGeocache> FoundGeocaches { get; set; }

    }
}
