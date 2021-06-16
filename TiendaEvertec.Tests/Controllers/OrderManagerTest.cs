using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlacetoPay.Integrations.Library.CSharp.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaEvertec.core.Business;
using TiendaEvertec.core.Entities;

namespace TiendaEvertec.Tests.Controllers
{

    

    [TestClass]
    public class OrderManagerTest
    {
        public int IdOrderTest = 76;
        public int IdRequestPlaceToPay = 1816022;

        [TestMethod]
        public void SaveOrder()
        {
            TiendaEvertec.core.Entities.Order order = new core.Entities.Order(0, null, "Juan", "Lopez", "Calle 1234",
                                                                               "32444222", "CC", "Empresa", "mail@mail.com", "541112341234", TiendaEvertec.core.Entities.OrderStatus.CREATED, DateTime.Now, null, 20000);
            List<Product> listaProductos = new List<Product>();
            listaProductos.Add(new Product() { ProductDescription = "Camiseta Deportiva L - DryFit", Quantity = 2, ValuePerUnit = 4000 });
            listaProductos.Add(new Product() { ProductDescription = "Pelota Nro 5 Adidas", Quantity = 1, ValuePerUnit = 5000 });

            OrdersManager orderManager = new OrdersManager();
            Order response = orderManager.SaveOrder(order,listaProductos);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void GetOrderList()
        {
           
            OrdersManager orderManager = new OrdersManager();
            List<core.Entities.Order> list= orderManager.GetOrderList();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void GetOrder()
        {
           
            OrdersManager orderManager = new OrdersManager();
            core.Entities.Order order = orderManager.GetOrder(IdOrderTest);
            Assert.IsNotNull(order);
        }


        [TestMethod]
        public void CreatePaymentRequest()
        {
            core.Entities.AdditionalOrderInfo additionalOrderInfo = new core.Entities.AdditionalOrderInfo("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.101 Safari/537.36", "127.0.0.1");
            OrdersManager orderManager = new OrdersManager();
            RedirectResponse response = orderManager.CreatePaymentRequest(IdOrderTest,additionalOrderInfo);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void UpdateOrder()
        {
            core.Entities.AdditionalOrderInfo additionalOrderInfo = new core.Entities.AdditionalOrderInfo("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.101 Safari/537.36", "127.0.0.1");
            OrdersManager orderManager = new OrdersManager();
            int response=orderManager.UpdateOrder(IdOrderTest, "PAYED");
            Assert.AreEqual(IdOrderTest, response);
        }

        [TestMethod]
        public void CheckPaymentStatus()
        {
           
            OrdersManager orderManager = new OrdersManager();
            string response = orderManager.CheckPaymentStatus(IdRequestPlaceToPay);
            Assert.AreEqual("PAYED", response);
        }


    }
}
