using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtorIugu.Models
{
    public class CobrancaDiretaRetorno
    {
        /// <summary>
        /// Esse retorno é válido tanto para credit_card quanto para bank_slip
        /// Os retornos exclusivos do credit_card estão comentados
        /// </summary>
        public string message { get; set; } //exclusivo credit_card
        public List<object> errors { get; set; } //exclusivo credit_card
        public bool success { get; set; }
        public string url { get; set; }
        public string pdf { get; set; }
        public string identification { get; set; }
        public string invoice_id { get; set; }
        public string LR { get; set; }
    }
}
