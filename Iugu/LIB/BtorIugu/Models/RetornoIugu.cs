using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BtorIugu.Models
{
    public partial class RetornoIugu : ExtratoDeFatura
    {
        public FaturaRetorno Facets { get; set; }
        public int TotalItems { get; set; }
        public List<ItensFatura> Items { get; set; }
        new public string created_at { get; set; }
        public DateTime created_at_iso { get; set; }
        public DateTime updated_at_iso { get; set; }
        new public string id { get; set; }
        public string currency { get; set; }
        public int discount_cents { get; set; }
        public int items_total_cents { get; set; }
        public string email { get; set; }
        public string notification_url { get; set; }
        public string return_url { get; set; }
        public string status { get; set; }
        public string tax_cents { get; set; }
        public int total_cents { get; set; }
        public int total_paid_cents { get; set; }
        public string taxes_paid_cents { get; set; }
        public string cc_emails { get; set; }
        public string financial_return_date { get; set; }
        public string payable_with { get; set; }
        public string overpaid_cents { get; set; }
        public bool ignore_due_email { get; set; }
        public string ignore_canceled_email { get; set; }
        public string secure_id { get; set; }
        public string secure_url { get; set; }
        public string total { get; set; }
        public string total_paid { get; set; }
        public string paid { get; set; }
        public string total_overpaid { get; set; }
        public string commission { get; set; }
        public string discount { get; set; }
        public List<Historico> logs { get; set; }
        public BoletoBancario bank_slip { get; set; }
    }
}