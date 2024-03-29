﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Geocaching.Models
{
    public class Geocache
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public Person Person { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [MaxLength(255)]
        public string Contents { get; set; }
        [MaxLength(255)]
        public string Message { get; set; }
        public List<FoundGeocache> FoundGeocaches { get; set; }



    }
}
