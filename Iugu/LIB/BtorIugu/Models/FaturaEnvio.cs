using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BtorIugu.Models
{
    [Serializable]
    public class FaturaEnvio
    {
        public string id { get; set; } //preenchido após a inclusão da fatura e pode ser utilizado para gerar a Segunda Via
        //public FaturaEnvio()
        //{
        //    ignore_due_email = true; //ignorar e-mail de cobrança --> usado em ambiente de testes.
        //}
        public string email { get; set; } //E-mail ao qual será enviada a fatura
        public string cc_emails { get; set; } //Endereços de E-mail para cópia separados por ponto e vírgula.
        public DateTime due_date { get; set; } //Data do vencimento. (Formato: 'AAAA-MM-DD').
        public bool ensure_workday_due_date { get; set; } // Se true, garante que a data de vencimento seja apenas em dias de semana, e não em sábados ou domingos.
        public int total_cents { get; set; } // Valor total da fatura (em centavos)
        public List<Produto> items { get; set; } //Itens da fatura.
        public bool fines { get; set; } //Habilitar multa por atraso de pagamento?
        public int late_payment_fine { get; set; } //Determina a multa % a ser cobrada para pagamentos efetuados após a data de vencimento
        public int discount_cents { get; set; } //Valor dos Descontos em centavos
        public string customer_id { get; set; } //ID do Cliente
        public bool ignore_due_email { get; set; } //Ignorar o envio do e-mail de cobrança?
        public string subscription_id { get; set; } //Amarra esta Fatura com a Assinatura especificada. Esta fatura não causa alterações na assinatura vinculada.
        public string payable_with { get; set; } //Método de pagamento que será disponibilizado para esta Fatura ("all", "credit_card" ou "bank_slip"). Obs: Caso esta Fatura esteja atrelada à uma Assinatura, a prioridade é herdar o valor atribuído na Assinatura; caso esta esteja atribuído o valor 'all', o sistema considerará o 'payable_with' da Fatura; se não, o sistema considerará o 'payable_with' da Assinatura.
        public Cliente payer { get; set; }
    }
}