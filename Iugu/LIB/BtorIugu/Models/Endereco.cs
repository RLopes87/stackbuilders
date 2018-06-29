using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BtorIugu.Models
{
    [Serializable]
    public class Endereco
    {
        public string zip_code { get; set; } //CEP
        public string street { get; set; }
        public string number { get; set; }
        public string district { get; set; } //Bairro
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string complement { get; set; }


    }
}