using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtorIugu.Models
{
    public class ExtratoDeFatura
    {
        /// <summary>
        /// Os campos abaixo são utilizados pelos métodos de consulta de faturas com status
        /// Como por exemplo: BuscarFaturasPagas()
        /// </summary>
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime due_date { get; set; }
        public DateTime? occurrence_date { get; set; }
        public DateTime? paid_at { get; set; }
        public string pending_value { get; set; }
        public string paid_value { get; set; }
        public string taxes_paid { get; set; }
        public string payment_method { get; set; }
        public string installments { get; set; }
        public string customer_id { get; set; }
        public string customer_email { get; set; }
        public string customer_name { get; set; }
        public string subscription_id { get; set; }
        public string receivable_date { get; set; }
        public string receivable_reference { get; set; }
        public string receivable_total { get; set; }
    }
}
