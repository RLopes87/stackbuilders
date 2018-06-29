using System;
using System.Collections.Generic;
using BtorIugu.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestIugu
{
    [TestClass]
    public class FaturaEnvioTest
    {
        [TestMethod]
        public void ObjetoPagadorInvalido()
        {
            FaturaEnvio fe = new FaturaEnvio();
            fe.payer = new Cliente();
            fe.payer.name = "Algum nome";
            fe.items = new List<Produto>();
            fe.items.Add(new Produto() { description = "Item teste 2", quantity = 1, price_cents = 100 });
            ValidarObjetoPagador(fe);
        }

        private void ValidarObjetoPagador(FaturaEnvio fe)
        {
            if (fe == null)
                throw new NullReferenceException("Fatura nula");
            if (fe.payer == null)
                throw new NullReferenceException("Pagador nulo");
            else if (string.IsNullOrEmpty(fe.payer.name))
                throw new NullReferenceException("Pagador nulo");
            else
                Assert.AreEqual("Algum nome", fe.payer.name);

            if (fe.items == null)
                throw new NullReferenceException("Itens nulo");
            else if (fe.items.Count == 0)
                throw new IndexOutOfRangeException("A fatura deve conter ao menos um produto");
            else
            {
                foreach (var produto in fe.items)
                    if (produto.price_cents < 100)
                        throw new Exception("O valor mínimo de cada produto é 100 (em centavos)");
                Assert.AreEqual(1, fe.items.Count);
            }
        }
    }
}
