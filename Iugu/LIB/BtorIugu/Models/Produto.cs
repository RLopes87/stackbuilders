using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BtorIugu.Models
{
    [Serializable]
    public class Produto
    {
        public string id { get; set; }
        public string description { get; set; }
        public int price_cents { get; set; } //Valor mínimo 100.
        public int quantity { get; set; }
        public string price { get; set; }


    }
}