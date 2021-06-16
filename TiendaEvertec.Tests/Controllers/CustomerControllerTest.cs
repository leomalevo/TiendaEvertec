using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TiendaEvertec.Controllers;

namespace TiendaEvertec.Tests.Controllers
{
    [TestClass]
    public class CustomerControllerTest
    {
        public int IdOrderTest = 76;


        [TestMethod]
        public void Index()
        {
            
            CustomerController controller = new CustomerController();
            ViewResult result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void FillOrderForm()
        {

            CustomerController controller = new CustomerController();
            ViewResult result = controller.FillOrderForm() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void FillOrderForm_Post()
        {


            Models.Order newOrder = new Models.Order {
                created_at = DateTime.Now,
                customer_address = "Calle 1234",
                customer_company = "Empresa",
                customer_document = "32456789",
                customer_documenttype = "CC",
                customer_email = "mail@mail.com",
                customer_mobile = "541145671234",
                customer_name = "Juan",
                customer_surname = "Perez",
                OrderCost = 20000,
                status = TiendaEvertec.core.Entities.OrderStatus.CREATED,
                Id = 0


            };
            core.Entities.AdditionalOrderInfo additionalOrderInfo = new core.Entities.AdditionalOrderInfo("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.101 Safari/537.36", "127.0.0.1");

            CustomerController controller = new CustomerController();
            ViewResult result = controller.FillOrderForm(newOrder) as ViewResult;
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void PreviewOrder()
        {
            CustomerController controller = new CustomerController();
            ViewResult result = controller.PreviewOrder(IdOrderTest) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ConfirmOrder()
        {
            CustomerController controller = new CustomerController();
            ViewResult result = controller.ConfirmOrder(IdOrderTest) as ViewResult;
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void OrderSaved()
        {
            CustomerController controller = new CustomerController();
            ViewResult result = controller.OrderSaved(IdOrderTest) as ViewResult;
            Assert.IsNotNull(result);
        }



        [TestMethod]
        public void ViewOrder()
        {
            CustomerController controller = new CustomerController();
            ViewResult result = controller.ViewOrder(IdOrderTest) as ViewResult;
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void OrderPendingOrRejected()
        {
            CustomerController controller = new CustomerController();
            ViewResult result = controller.OrderPendingOrRejected(IdOrderTest) as ViewResult;
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void ResendOrder()
        {
            CustomerController controller = new CustomerController();
            ViewResult result = controller.ReSendOrder(IdOrderTest) as ViewResult;
            Assert.IsNotNull(result);
        }

    }
}
