using BtorIugu.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BtorIugu
{
    public class Iugu : IDisposable
    {
        private string TokenIugu { get; set; }
        private RetornoIugu FaturaIugu;
        private List<RetornoIugu> FaturasIugu = new List<RetornoIugu>();
        private List<ExtratoDeFatura> ExtratoDeFaturas = new List<ExtratoDeFatura>();
        private WebRequest Request;
        private bool Disposed;

        #region Construtor e Montar Requisição
        public Iugu(string tokenIugu)
        {
            this.TokenIugu = tokenIugu;
        }
        /// <summary>
        /// Método que faz a autenticação em base 64 (conforme Iugu) e faz o apontamento para o método que será consumido da API da Iugu.
        /// </summary>
        /// <param name="verbo">GET ou POST</param>
        /// <param name="url">endereço https. Obs: método/verbo com parâmetros devem ser incluídos na própria url</param>
        /// <returns></returns>
        private WebRequest GetRequest(string verbo, string url)
        {
            string codificacaoBase64 = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(TokenIugu + ":"));
            WebRequest request = WebRequest.Create(url);
            request.Headers.Add("Authorization", "Basic " + codificacaoBase64);
            request.ContentType = "application/json";
            request.Method = verbo;
            return request;
        }
        #endregion

        #region Faturas
        /// <summary>
        /// Retorna uma fatura em específico.
        /// </summary>
        /// <param name="idFatura">(Opcional) Código da fatura a ser consultada.</param>
        public object CriarFatura(FaturaEnvio dadosFatura) // No final da classe tem um exemplo do objeto FaturaEnvio populado dentro do método MontarFaturaEnvio. (testado e funcionando)
        {
            object retornoIugu;
            try
            {
                Request = GetRequest("POST", @"https://api.iugu.com/v1/invoices/");
                using (StreamWriter sw = new StreamWriter(Request.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(dadosFatura);
                    sw.Write(json);
                    sw.Flush();
                }
                using (HttpWebResponse httpResponse = (HttpWebResponse)Request.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        retornoIugu = JObject.Parse(responseText).ToString();
                    }
                }
                return retornoIugu;
            }
            catch (WebException we)
            {
                var erroIugu = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                dynamic json = JsonConvert.DeserializeObject(erroIugu);
                return new { erro = true, erroOriginal = ((HttpWebResponse)we.Response).StatusDescription, descricaoErro = ReplacePropriedadesErro(json) };
            }
        }
        /// <summary>
        /// Gera segunda via de uma Fatura. Somente faturas pendentes podem ter segunda via gerada. 
        /// A fatura atual é cancelada e uma nova é gerada com status "pendente" / "pending"
        /// *** Apenas fatura com o status pendente são passíveis de segunda via ***
        /// **** ATENÇÃO: na emissão de segunda via, somente o ID e a data de vencimento são alterados, não há a alteração 
        /// de registros como o valor total "total_cents" e o desconto "discount_cents" ****
        /// </summary>
        /// <param name="dadosFatura">As propriedades "id" e "due_date" são obrigatórias para esta transação</param>
        /// <returns></returns>
        public dynamic SegundaVia(string idIugu, DateTime dataAlteracao)//, int valorEmCentavos, int descontoEmCentavos)
        {
            dynamic objetoJS = new { id = idIugu, due_date = dataAlteracao.ToString("yyyy-MM-dd") };//, total_cents = valorEmCentavos, discount_cents = descontoEmCentavos };
            dynamic novaFatura;
            try
            {
                Request = GetRequest("POST", @"https://api.iugu.com/v1/invoices/" + idIugu + "/duplicate");
                using (StreamWriter sw = new StreamWriter(Request.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(objetoJS);
                    sw.Write(json);
                    sw.Flush();
                }
                using (HttpWebResponse httpResponse = (HttpWebResponse)Request.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        novaFatura = JObject.Parse(responseText).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                novaFatura = "Não foi possível efetuar transação. Erro original: " + ex.Message;
            }
            return novaFatura;
        }
        public dynamic BuscarFatura(string idFatura)
        {
            try
            {
                Request = GetRequest("GET", @"https://api.iugu.com/v1/invoices/" + idFatura);
                using (var retorno = Request.GetResponse())
                {
                    string json = new StreamReader(retorno.GetResponseStream()).ReadToEnd();
                    FaturaIugu = JsonConvert.DeserializeObject<RetornoIugu>(json);
                }
            }
            catch (WebException we)
            {
                HttpWebResponse resposta = we.Response as HttpWebResponse;
                return new { erro = true, erroOriginal = resposta.StatusDescription, descricaoErro = "Erro ao tentar localizar a fatura com ID: " + idFatura };
            }
            return FaturaIugu;
        }
        /// <summary>
        /// Retorna todas as faturas deste "customer_id".
        /// </summary>
        /// <param name="customer_id">Código da cliente emissor das faturas.</param>
        public RetornoIugu BuscarTodasFaturas(string customer_id)
        {
            Request = GetRequest("GET", @"https://api.iugu.com/v1/invoices/");
            using (var retorno = Request.GetResponse())
            {
                string json = new StreamReader(retorno.GetResponseStream()).ReadToEnd();
                FaturaIugu = JsonConvert.DeserializeObject<RetornoIugu>(json);
            }
            FaturaIugu.Items.RemoveAll(c => c.Customer_Id != customer_id);
            return FaturaIugu;
        }
        /// <summary>
        /// Retorna todas as faturas com status "paid" independente do "customer_id" considerando a data de pagamento "paid_at" nos 3 últimos dias a contar de D-1.
        /// </summary>
        public List<ExtratoDeFatura> BuscarFaturasPagas()
        {
            DateTime dtInicio = DateTime.Now.AddDays(-20);

            ExtratoDeFaturas = BuscarFaturasPorStatus("paid");
            if (ExtratoDeFaturas.Count != 0)
            {
                var dados = ExtratoDeFaturas.Where(c => c.paid_at.Value.Date >= dtInicio).ToList();

                //var sw = new System.IO.StreamWriter("C:/TESTE/TESTE.TXT", true);
                //sw.WriteLine("Encontrou " + dados.Count.ToString());
                //sw.Close();

                return dados;
            }
            return ExtratoDeFaturas;
        }
        /// <summary>
        /// Retorna todas as faturas com status "paid" do "customer_id" considerando a data de pagamento "paid_at" nos 3 últimos dias a contar de D-1.
        /// </summary>
        /// <param name="customer_id">Identificador do cliente na base de dados do Iugu.</param>
        public List<ExtratoDeFatura> BuscarFaturasPagas(string customer_id)
        {
            //DateTime dMenos1 = DateTime.Now.AddDays(-1);
            DateTime dtInicio = DateTime.Now.AddDays(-20);

            ExtratoDeFaturas = BuscarFaturasPorStatus("paid");
            if (ExtratoDeFaturas.Count != 0)
                //return ExtratoDeFaturas.Where(c => c.customer_id == customer_id && (c.paid_at.Value.Date <= dMenos1.Date && c.paid_at.Value.Date >= dMenos1.Date.AddDays(-2))).ToList();
                return ExtratoDeFaturas.Where(c => c.customer_id == customer_id && (c.paid_at.Value.Date >= dtInicio)).ToList();
            return ExtratoDeFaturas;
        }
        /// <summary>
        /// Retorna todas as faturas com status "paid" do "customer_id" considerando a data de pagamento "paid_at" entre "dataInicio" e "dataFim".
        /// </summary>
        /// <param name="customer_id">Identificador do cliente na base de dados do Iugu.</param>
        /// <param name="dataInicio">Data Início de filtro dos pagamentos registrados na base de dados do Iugu.</param>
        /// <param name="dataFim">Data Fim de filtro dos pagamentos registrados na base de dados do Iugu.</param>
        public List<ExtratoDeFatura> BuscarFaturasPagas(string customer_id, DateTime dataInicio, DateTime dataFim)
        {
            ExtratoDeFaturas = BuscarFaturasPorStatus("paid");
            if (ExtratoDeFaturas.Count != 0)
                return ExtratoDeFaturas.Where(c => c.customer_id == customer_id && (c.paid_at.Value.Date >= dataInicio.Date && c.paid_at.Value.Date <= dataFim.Date)).ToList();
            return ExtratoDeFaturas;
        }
        /// <summary>
        /// Retorna todas as faturas com status "pending" do "customer_id".
        /// </summary>
        /// <param name="customer_id">Identificador do cliente na base de dados do Iugu.</param>
        public List<ExtratoDeFatura> BuscarFaturasPendentes(string customer_id)
        {
            ExtratoDeFaturas = BuscarFaturasPorStatus("pending");
            if (ExtratoDeFaturas.Count != 0)
                return ExtratoDeFaturas.Where(c => c.customer_id == customer_id).ToList();
            return ExtratoDeFaturas;
        }
        /// <summary>
        /// Retorna todas as faturas com status "pending" do "customer_id" considerando a data de vencimento "due_date" entre "dataInicio" e "dataFim".
        /// </summary>
        /// <param name="customer_id">Identificador do cliente na base de dados do Iugu.</param>
        /// <param name="dataInicio">Data Início de filtro dos pagamentos registrados na base de dados do Iugu.</param>
        /// <param name="dataFim">Data Fim de filtro dos pagamentos registrados na base de dados do Iugu.</param>
        public List<ExtratoDeFatura> BuscarFaturasPendentes(string customer_id, DateTime dataInicio, DateTime dataFim)
        {
            ExtratoDeFaturas = BuscarFaturasPorStatus("pending");
            if (ExtratoDeFaturas.Count != 0)
                return ExtratoDeFaturas.Where(c => c.customer_id == customer_id && (c.due_date.Date >= dataInicio.Date && c.due_date.Date <= dataFim.Date)).ToList();
            return ExtratoDeFaturas;
        }
        /// <summary>
        /// Retorna todas as faturas com status "partially_paid" do "customer_id".
        /// </summary>
        /// <param name="customer_id">Identificador do cliente na base de dados do Iugu.</param>
        public List<ExtratoDeFatura> BuscarFaturasComPagamentoParcial(string customer_id)
        {
            ExtratoDeFaturas = BuscarFaturasPorStatus("partially_paid");
            if (ExtratoDeFaturas.Count != 0)
                return ExtratoDeFaturas.Where(c => c.customer_id == customer_id).ToList();
            return ExtratoDeFaturas;
        }
        /// <summary>
        /// Retorna todas as faturas com status "partially_paid" do "customer_id" considerando a data de pagamento "paid_at" entre "dataInicio" e "dataFim".
        /// </summary>
        /// <param name="customer_id">Identificador do cliente na base de dados do Iugu.</param>
        /// <param name="dataInicio">Data Início de filtro dos pagamentos registrados na base de dados do Iugu.</param>
        /// <param name="dataFim">Data Fim de filtro dos pagamentos registrados na base de dados do Iugu.</param>
        public List<ExtratoDeFatura> BuscarFaturasComPagamentoParcial(string customer_id, DateTime dataInicio, DateTime dataFim)
        {
            ExtratoDeFaturas = BuscarFaturasPorStatus("partially_paid");
            if (ExtratoDeFaturas.Count != 0)
                return ExtratoDeFaturas.Where(c => c.customer_id == customer_id && (c.paid_at.Value.Date >= dataInicio.Date && c.paid_at.Value.Date <= dataFim.Date)).ToList();
            return ExtratoDeFaturas;
        }
        /// <summary>
        /// Retorna todas as faturas com status "canceled" do "customer_id".
        /// </summary>
        /// <param name="customer_id">Identificador do cliente na base de dados do Iugu.</param>
        public List<ExtratoDeFatura> BuscarFaturasCanceladas(string customer_id)
        {
            ExtratoDeFaturas = BuscarFaturasPorStatus("canceled");
            if (ExtratoDeFaturas.Count != 0)
                return ExtratoDeFaturas.Where(c => c.customer_id == customer_id).ToList();
            return ExtratoDeFaturas;
        }
        /// <summary>
        /// Retorna todas as faturas com status "canceled" do "customer_id" considerando a data de criação "created_at" entre "dataInicio" e "dataFim".
        /// </summary>
        /// <param name="customer_id">Identificador do cliente na base de dados do Iugu.</param>
        /// <param name="dataInicio">Data Início de filtro dos pagamentos registrados na base de dados do Iugu.</param>
        /// <param name="dataFim">Data Fim de filtro dos pagamentos registrados na base de dados do Iugu.</param>
        public List<ExtratoDeFatura> BuscarFaturasCanceladas(string customer_id, DateTime dataInicio, DateTime dataFim)
        {
            ExtratoDeFaturas = BuscarFaturasPorStatus("canceled");
            if (ExtratoDeFaturas.Count != 0)
                return ExtratoDeFaturas.Where(c => c.customer_id == customer_id && (c.created_at.Date >= dataInicio.Date && c.created_at.Date <= dataFim.Date)).ToList();
            return ExtratoDeFaturas;
        }
        private dynamic BuscarFaturasPorStatus(string status)
        {
            Request = GetRequest("GET", @"https://api.iugu.com/v1/accounts/invoices?status=" + status);
            List<ExtratoDeFatura> retornoFaturas = new List<ExtratoDeFatura>();
            using (WebResponse retorno = Request.GetResponse())
            {
                string json = new StreamReader(retorno.GetResponseStream()).ReadToEnd();
                retornoFaturas = JsonConvert.DeserializeObject<List<ExtratoDeFatura>>(json);
            }
            return retornoFaturas;
        }
        public string CancelarFatura(string idFatura)
        {
            string idIugu = string.Empty;
            try
            {
                Request = GetRequest("PUT", @"https://api.iugu.com/v1/invoices/" + idFatura + "/cancel");
                using (HttpWebResponse httpResponse = (HttpWebResponse)Request.GetResponse())
                {
                    dynamic respostaIugu;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        respostaIugu = JObject.Parse(responseText);
                    }
                    idIugu = respostaIugu.id;
                }
            }
            catch (Exception ex)
            {
                idIugu = "Não foi possível efetuar transação. Erro Original: " + ex.Message;
            }
            return idIugu;
        }
        #endregion

        #region Cobranças Diretas

        public dynamic CriarTokenCobrancaDireta(Token token)
        {
            object tokenRetorno;
            try
            {
                Request = GetRequest("POST", @"https://api.iugu.com/v1/payment_token");
                using (StreamWriter sw = new StreamWriter(Request.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(token);
                    sw.Write(json);
                    sw.Flush();
                }
                using (HttpWebResponse httpResponse = (HttpWebResponse)Request.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        tokenRetorno = JObject.Parse(responseText).ToString();
                        var js = JsonConvert.DeserializeObject(tokenRetorno.ToString());
                    }
                }
                var ret = JsonConvert.DeserializeObject(tokenRetorno.ToString());
                return ((dynamic)ret).id;
            }
            catch (WebException we)
            {
                var erroIugu = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                dynamic json = JsonConvert.DeserializeObject(erroIugu);
                return new { erro = true, erroOriginal = ((HttpWebResponse)we.Response).StatusDescription, descricaoErro = ReplacePropriedadesErro(json).ToString().Replace("|", "") };
            }
        }

        public dynamic GerarCobrancaDireta(CobrancaDireta cobrancaDireta)
        {
            object retornoIugu;
            try
            {
                Request = GetRequest("POST", @"https://api.iugu.com/v1/charge");
                using (StreamWriter sw = new StreamWriter(Request.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(cobrancaDireta);
                    sw.Write(json);
                    sw.Flush();
                }
                using (HttpWebResponse httpResponse = (HttpWebResponse)Request.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        retornoIugu = JObject.Parse(responseText).ToString();
                    }
                }
                return retornoIugu;
            }
            catch (WebException we)
            {
                var erroIugu = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                dynamic json = JsonConvert.DeserializeObject(erroIugu);
                return new { erro = true, erroOriginal = ((HttpWebResponse)we.Response).StatusDescription, descricaoErro = ReplacePropriedadesErro(json) };
            }
        }

        public dynamic BaixarFatura(string idFatura)
        {
            try
            {
                Request = GetRequest("POST", @"https://api.iugu.com/v1/invoices/" + idFatura + "/capture");
                using (var retorno = Request.GetResponse())
                {
                    string json = new StreamReader(retorno.GetResponseStream()).ReadToEnd();
                    FaturaIugu = JsonConvert.DeserializeObject<RetornoIugu>(json);
                }
            }
            catch (WebException we)
            {
                HttpWebResponse resposta = we.Response as HttpWebResponse;
                return new { erro = true, erroOriginal = resposta.StatusDescription, descricaoErro = "Erro ao tentar baixar a fatura com ID: " + idFatura };
            }
            return FaturaIugu;
        }
        #endregion

        #region Clientes
        public dynamic CriarCliente(Cliente cliente)
        {
            dynamic retornoIugu;
            try
            {
                Request = GetRequest("POST", @"https://api.iugu.com/v1/customers/");
                using (StreamWriter sw = new StreamWriter(Request.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(cliente);
                    sw.Write(json);
                    sw.Flush();
                }
                using (HttpWebResponse httpResponse = (HttpWebResponse)Request.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        retornoIugu = JObject.Parse(responseText).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                retornoIugu = "Não foi possível efetuar transação. Erro original: " + ex.Message;
            }
            return retornoIugu;
        }
        #endregion

        #region Destructor
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    if (FaturaIugu != null)
                        FaturaIugu = null;
                    if (Request != null)
                        Request = null;
                }
            }
            Disposed = true;
        }
        #endregion

        #region Utilitários
        private string ReplacePropriedadesErro(dynamic json)
        {
            var msgRetorno = JsonConvert.SerializeObject(json.errors);
            msgRetorno = msgRetorno
                .Replace("{", "")
                .Replace("}", "")
                .Replace("[", "")
                .Replace("],", " | ")
                .Replace("]", " | ")
                .Replace("email", "E-mail")
                .Replace("due_date", "Data de Vencimento")
                .Replace("base", "Itens")
                .Replace("total", "Valor Total")
                .Replace("payer.cpf_cnpj", "CPF ou CNPJ do Pagador")
                .Replace("payer.name", "Nome do Pagador")
                .Replace("payer.address.zip_code", "CEP do Pagador")
                .Replace("payer.address.number", "Número Logradouro Pagador")
                .Replace("number", "Número")
                .Replace("is invalid", "não é válido")
                .Replace("\":\"", " ");
            return msgRetorno;
        }

        #endregion

        #region Exemplos (Não consumir estes métodos - são apenas para exemplificar o preenchimento dos objetos)

        /// Exemplo de preenchimento do objeto FaturaEnvio
        /// Não deve ser usado.
        private static FaturaEnvio MontarFaturaEnvio()
        {
            FaturaEnvio fe = new FaturaEnvio();
            fe.email = "teste@teste.com.br";
            fe.cc_emails = "teste@teste.com.br";
            fe.due_date = DateTime.Now.Date;
            fe.fines = false;
            fe.discount_cents = 0;
            fe.customer_id = "customer id aqui"; //AR PRIME
            fe.ignore_due_email = false;

            fe.items = new List<Produto>();
            fe.items.Add(new Produto() { description = "Item teste 1", quantity = 1, price_cents = 100 }); //, Price = "R$ 1,00" });
            fe.items.Add(new Produto() { description = "Item teste 2", quantity = 1, price_cents = 100 }); //, Price = "R$ 2,00" });

            fe.payer = new Cliente();
            fe.payer.cpf_cnpj = "98765432100";
            fe.payer.name = "Pagador Teste";
            fe.payer.phone_prefix = "65";
            fe.payer.phone = "123456789";
            fe.payer.email = "teste@teste.com.br";

            fe.payer.address = new Endereco();
            fe.payer.address.zip_code = "78040365";
            fe.payer.address.street = "Av. Miguel Sutil";
            fe.payer.address.number = "8388";
            fe.payer.address.district = "Santa Rosa";
            fe.payer.address.city = "Cuiabá";
            fe.payer.address.state = "Mato Grosso";
            fe.payer.address.country = "Brasil";
            fe.payer.address.complement = "Salas 908/909";
            return fe;
        }

        //Exemplo de consumo do método ListarFaturas(na aplicação cliente)
        //    RetornoIugu faturas = new RetornoIugu();
        //    using (Iugu iugu = new Iugu("40dc7d532b37ebbad6adca95d3e3375e"))
        //        faturas = iugu.ListarFaturas(); 

        /// <summary>
        /// O objeto TOKEN deve ser populado com os dados do cartão e, para isso, é necessário informar o código do ID da Configuração da conta titular.
        /// </summary>
        /// <param name="idContaTitularIugu">000000000000000000000000000000 para AR Prime</param>
        /// <returns></returns>
        private Token MontarToken(string idContaTitularIugu)
        {
            Token token = new Token();
            token.account_id = idContaTitularIugu;
            token.method = "credit_card"; //"all", bank_slip
            token.test = true; 

            token.data = new DadosCartao();
            token.data.number = "4111111111111111"; //outros cartões teste: 4242424242424242; 5555555555554444
            token.data.verification_value = "987";
            token.data.first_name = "Qualquer Nome";
            token.data.last_name = "Qualquer Sobrenome";
            token.data.month = "12";
            token.data.year = "2020";

            return token;
        }
        private CobrancaDireta MontarCobrancaDireta(string token)
        {
            CobrancaDireta cd = new CobrancaDireta();
            cd.token = token;
            cd.customer_id = "customer_id aqui";
            cd.invoice_id = null;
            cd.email = "rodrigo@teste.com.br";
            cd.months = 2;
            cd.discount_cents = 100;
            cd.bank_slip_extra_days = 0;
            cd.keep_dunning = true;

            cd.items = new List<Produto>();
            cd.items.Add(new Produto() { description = "PRODUTO TESTE 1", quantity = 1, price_cents = 1000 }); //, Price = "R$ 1,00" });

            cd.payer = new Cliente();
            cd.payer.cpf_cnpj = "28402903835";
            cd.payer.name = "USUÁRIO TESTE";
            cd.payer.phone_prefix = "65";
            cd.payer.phone = "99647-3769";
            cd.payer.email = "rodrigo@teste.com.br";

            cd.payer.address = new Endereco();
            cd.payer.address.zip_code = "04047-104";
            cd.payer.address.street = "Rua";
            cd.payer.address.number = "1";
            cd.payer.address.district = "Bairro";
            cd.payer.address.city = "Cidade";
            cd.payer.address.state = "Estado";
            cd.payer.address.country = "Brasil";
            cd.payer.address.complement = "Complemento";

            return cd;
        }
        #endregion
    }
}