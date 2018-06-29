using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtorIugu.Models
{
    public class Token
    {
        public string account_id { get; set; } //ID de sua Conta na Iugu: para conferir acessar: https://app.iugu.com/account
        public string method { get; set; } // Método de Pagamento (atualmente somente credit_card)
        public bool test { get; set; } // Valor true para criar tokens de teste. Para acessar a lista de cartões teste clique em: https://support.iugu.com/hc/pt-br/articles/212456346-Usar-cart%C3%B5es-de-teste-em-modo-de-teste
        public DadosCartao data { get; set; }
    }

    public class DadosCartao
    {
        public string number { get; set; } // Número do Cartão de Crédito
        public string verification_value { get; set; } // CVV do Cartão de Crédito
        public string first_name { get; set; } // Nome do Cliente como está no Cartão
        public string last_name { get; set; } // Sobrenome do Cliente como está no Cartão
        public string month { get; set; } // Mês de Vencimento no Formato "MM" (Ex: 01, 06, 12)
        public string year { get; set; } // Ano de Vencimento no Formato "AAAA" (Ex: 2020, 2030, 2018)
    }
}
