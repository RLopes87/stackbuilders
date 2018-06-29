using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtorIugu.Models
{
    public class CobrancaDireta
    {
        //public string method { get; set; } // método de pagamento (atualemente só suporta 'bank_slip', que é o boleto bancário. Não é preenchido se enviar o parâmetro token)
        public string token { get; set; } // ID do Token. Não é preenchido caso o método de pagamento seja 'bank_slip'. Em caso de Marketplace, é possível enviar um token criado pela conta mestre.
        //public string customer_payment_method_id { get; set; } // ID da Forma de Pagamento do Cliente. Em caso de Marketplace, é possível enviar um "customer_payment_method_id" de um Cliente criado pela conta mestre (não é preenchido caso Método de Pagamento seja "bank_slip" ou utilize "token")
        public bool restrict_payment_method { get; set; } //Se true, restringe o método de pagamento da cobrança para o definido em method
        public string customer_id { get; set; } //ID do Cliente. Utilizado para vincular a Fatura a um Cliente
        public string invoice_id { get; set; } //ID da Fatura a ser utilizada para pagamento
        public string email { get; set; } // E-mail do Cliente (não é preenchido caso seja enviado um "invoice_id")
        public int months { get; set; } // Número de Parcelas (2 até 12), não é necessário passar 1. Não é preenchido caso o método de pagamento seja "bank_slip"
        public int discount_cents { get; set; } // Valor dos Descontos em centavos. Funciona apenas para Cobranças Diretas criadas com Itens.
        public int bank_slip_extra_days { get; set; } // Define o prazo em dias para o pagamento do boleto. Caso não seja enviado, aplica-se o prazo padrão de 3 dias corridos.
        public bool keep_dunning { get; set; } // Por padrão, a fatura é cancelada caso haja falha na cobrança, a não ser que este parâmetro seja enviado como "true". Obs: Funcionalidade disponível apenas para faturas criadas no momento da cobrança.
        public List<Produto> items { get; set; } // Itens de cobrança da Fatura que será gerada. "price_cents" valor mínimo 100.
        public Cliente payer { get; set; } // Informações do cliente "payer" são obrigatórias para a emissão de boletos registrados ou necessárias para o sistema de antifraude.
    }
}
