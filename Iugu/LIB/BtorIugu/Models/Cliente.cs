using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BtorIugu.Models
{
    [Serializable]
    public class Cliente
    {
        public string id { get; set; } //preenchido no cadastro do cliente
        public string name { get; set; }
        public string email { get; set; }
        public string notes { get; set; } //Anotações Gerais
        public string cpf_cnpj { get; set; } //Obrigatório para emissão de boletos registrados
        public string cc_emails { get; set; } //Endereços de E-mail para cópia separados por vírgula
        public string zip_code { get; set; } //CEP. Obrigatório para emissão de boletos registrados
        public int number { get; set; } //Número do endereço(obrigatório caso "zip_code" seja enviado).
        public string street { get; set; } //Rua. Obrigatório caso CEP seja incompleto.
        public string city { get; set; }
        public string state { get; set; }
        public string district { get; set; } //Bairro. Obrigatório caso CEP seja incompleto.
        public string complement { get; set; } //Complemento de endereço. Ponto de referência.
        public string phone_prefix { get; set; } //Prefixo do telefone (Ex: 65 para Cuiabá)
        public string phone { get; set; }
        public Endereco address { get; set; }
    }
}