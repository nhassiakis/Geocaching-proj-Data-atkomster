using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maps.MapControl.WPF;


namespace Geocaching.Models
{
    public class Pin : Pushpin
    {
        public Geocache Geocache { get; set; }
        public Person Person { get; set; }

    }
}
