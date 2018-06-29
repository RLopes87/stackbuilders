using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtorIugu.Models
{
    class Erro
    {
        private const string Default = "Erro não localizado em nossa base de conhecimentos. Entre em contato com os desenvolvedores do sistema.";
        private const string Email =  "O e-mail deve ser válido.";
        private const string Data_Cobranca = "A data de cobrança não pode estar no passado.";
        private const string Itens = "Faturas pendentes necessitam de pelo menos um item.";
        private const string Total = "O total deve ser maior que 1.";
        private const string CPF_CNPJ_Pagador = "O CPF/CNPJ não pode ficar em branco.";
        private const string Nome_Pagador =  "O nome do pagador não pode ficar em branco.";
        private const string CEP_Endereco_Pagador = "O CEP do pagador não é válido.";
        private const string Numero_Endereco_Pagador = "O número do endereço do pagador não pode ficar em branco.";
        public enum Erros
        {
            Email = 1,
            Data_Cobranca = 2,
            Itens = 3,
            Total = 4,
            CPF_CNPJ_Pagador = 5,
            Nome_Pagador = 6,
            CEP_Endereco_Pagador = 7,
            Numero_Endereco_Pagador = 8
        }

        public static string GetErrorMessage(Erros codigoErro)
        {
            string errorMessage = string.Empty;
            switch (codigoErro)
            {
                case Erros.Email:
                    errorMessage = Email;
                    break;
                case Erros.Data_Cobranca:
                    errorMessage = Data_Cobranca;
                    break;
                case Erros.Itens:
                    errorMessage = Itens;
                    break;
                case Erros.Total:
                    errorMessage = Total;
                    break;
                case Erros.CPF_CNPJ_Pagador:
                    errorMessage = CPF_CNPJ_Pagador;
                    break;
                case Erros.Nome_Pagador:
                    errorMessage = Nome_Pagador;
                    break;
                case Erros.CEP_Endereco_Pagador:
                    errorMessage = CEP_Endereco_Pagador;
                    break;
                case Erros.Numero_Endereco_Pagador:
                    errorMessage = Numero_Endereco_Pagador;
                    break;
                default:
                    errorMessage = Default;
                    break;
            }
            return errorMessage;
        }
    }
}
