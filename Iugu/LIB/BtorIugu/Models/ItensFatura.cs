using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BtorIugu.Models
{
    public class ItensFatura
    {
        public string Id { get; set; }
        public DateTime Due_Date { get; set; }
        public string Currency { get; set; }
        public decimal? Discount_Cents { get; set; }
        public string Email { get; set; }
        public string Notification_Url { get; set; }
        public string Return_Url { get; set; }
        public string Status { get; set; }
        public decimal? Tax_Cents { get; set; }
        public DateTime Updated_At { get; set; }
        public decimal Total_Cents { get; set; }
        public decimal Total_Paid_Cents { get; set; }
        public DateTime? Paid_At { get; set; }
        public decimal? Taxes_Paid_Cents { get; set; }
        public decimal? Paid_Cents { get; set; }
        public string CC_Emails { get; set; }
        public string Payable_With { get; set; }
        public decimal? Overpaid_Cents { get; set; }
        public string Ignore_Due_Email { get; set; }
        public string Ignore_Canceled_Email { get; set; }
        public string Advance_Fee_Cents { get; set; }
        public string Commission_Cents { get; set; }
        public bool Early_Payment_Discount { get; set; }
        public string Secure_Id { get; set; }
        public string Secure_Url { get; set; }
        public string Customer_Id { get; set; }
        public string Customer_Ref { get; set; }
        public string Customer_Name { get; set; }
        public string User_Id { get; set; }
        public string Total { get; set; }
        public string Taxes_Paid { get; set; }
        public string Total_Paid { get; set; }
        public string Total_Overpaid { get; set; }
        public string Commission { get; set; }
        public string Fines_On_Occurrence_Day { get; set; }
        public string Total_On_Occurrence_Day { get; set; }
        public string Fines_On_Occurrence_Day_Cents { get; set; }
        public string Total_On_Occurrence_Day_Cents { get; set; }
        public DateTime? Financial_Return_Date { get; set; }
        public string Advance_Fee { get; set; }
        public string Paid { get; set; }
        public int Transaction_Number { get; set; }
        public string Payment_Method { get; set; }
        public List<Produto> Items { get; set; }
        public List<Variavel> Variables { get; set; }
        public List<Log> Logs { get; set; }

    }
}