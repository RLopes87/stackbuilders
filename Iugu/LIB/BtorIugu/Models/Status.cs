using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BtorIugu.Models
{
    public class Status
    {
        public string Missing { get; set; }
        public string Total { get; set; }
        public string Others { get; set; }
        public List<Termos> Terms { get; set; }
    }
}