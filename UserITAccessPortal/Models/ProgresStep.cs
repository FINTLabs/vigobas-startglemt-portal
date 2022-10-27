using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VigoBAS_Start.Models
{
    public class ProgresStep
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public bool Current { get; set; }
    }
}