using PlacetoPay.Integrations.Library.CSharp.Contracts;
using PlacetoPay.Integrations.Library.CSharp.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaEvertec.core.Data;
using TiendaEvertec.core.Entities;
using PlacetoPay.Integrations.Library.CSharp;
using PlacetoPay.Integrations.Library.CSharp.Message;
using Mapster;
using System.Data.Entity;

namespace TiendaEvertec.core.Business
{
    public class OrdersManager
    {

        public string PlaceToPay_Login = ConfigurationManager.AppSettings["PlaceToPay_Login"];
        public string PlaceToPay_TranKey = ConfigurationManager.AppSettings["PlaceToPay_TranKey"];
        public string PlaceToPay_URL = ConfigurationManager.AppSettings["PlaceToPay_URL"];
        public string URLToReturn = ConfigurationManager.AppSettings["URLToReturn"];
        public string IPServicio = ConfigurationManager.AppSettings["IPServicio"];

        public string Currency = ConfigurationManager.AppSettings["Currency"];
        public string CurrencySymbol = ConfigurationManager.AppSettings["CurrencySymbol"];

        private static OrdersManager _instance;
        private static object syncLock = new object();

        public static OrdersManager GetInstance()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new OrdersManager();
                    }
                }
            }

            return _instance;
        }



        /// <summary>
        /// Consulta la orden mediante un ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Entities.Order GetOrder(int Id)
        {
            Entities.Order order = null;
            try
            {
                using (var db = new TiendaEvertecEntities())
                {

                    var orderDB = db.orders.SingleOrDefault(b => b.Id == Id);
                    order = orderDB.Adapt<Order>();
                }


                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("SaverOrder: " + ex.Message);
            }

        }


        /// <summary>
        /// Inicia la Request de pago necesario para PlaceToPay
        /// </summary>
        /// <param name="Id">Id de la Orden</param>
        /// <param name="info">Informacion adicional para la request de PlaceToPay</param>
        /// <returns></returns>
        public RedirectResponse CreatePaymentRequest(int Id, AdditionalOrderInfo info)
        {
           
            try
            {
                using (var db = new TiendaEvertecEntities())
                {
                    var orderDB = db.orders.SingleOrDefault(b => b.Id == Id);
                    var order = orderDB.Adapt<Order>();
                    //inicia la cracion de la request en PLaceToPay
                    RedirectResponse response = GetRequestFromPlaceToPay(order,info);
                    
                    //actualiza la DB con el IdRequest
                    orderDB.IdRequest = Convert.ToInt32(response.RequestId);
                    db.SaveChanges();

                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveOrderAndCreatePaymentRequest: " + ex.Message);
            }

        }

        /// <summary>
        /// Guarda la Orden en la DB
        /// </summary>
        /// <param name="order">Los datos de la orden</param>
        /// <param name="Products">Los productos comprados por el cliente y que se asocian a la orden</param>
        /// <returns></returns>
        public Order SaveOrder(Order order, List<Product> Products)
        {
            DbContextTransaction dbContextTransaction = null;
            try
            {
                using (var db = new TiendaEvertecEntities())
                {

                    using (dbContextTransaction = db.Database.BeginTransaction())
                    {
                        var newOrder = order.Adapt<orders>();
                        newOrder.status = OrderStatus.CREATED;
                        newOrder.created_at = DateTime.Now;
                        newOrder.updated_at = newOrder.created_at;
                        newOrder.OrderCost = Products.Sum(x => x.Quantity * x.ValuePerUnit);
                        db.Set<orders>().Add(newOrder);
                        db.SaveChanges();
                        order.Id = newOrder.Id;


                        foreach (Product newProduct in Products)
                        {
                            var newProductDB = newProduct.Adapt<orderProducts>();
                            newProductDB.IdOrder = newOrder.Id;
                            db.Set<orderProducts>().Add(newProductDB);
                            db.SaveChanges();
                        }

                        dbContextTransaction.Commit();

                        order = newOrder.Adapt<Order>();
                        return order;
                    }
                }
            }
            catch (Exception ex)
            {
                if (dbContextTransaction != null)
                    dbContextTransaction.Rollback();

                throw new Exception("SaverOrder: " + ex.Message);

            }

        }

        /// <summary>
        /// Actualizan el estado de la orden
        /// </summary>
        /// <param name="Id">La orden a actualizar</param>
        /// <param name="status">Estado nuevo de la orden</param>
        public int UpdateOrder(int Id, string status)
        {
            try
            {
                using (var db = new TiendaEvertecEntities())
                {
                    var order = db.orders.Where(x=>x.Id==Id).FirstOrDefault();
                    order.status = status;
                    order.updated_at = DateTime.Now;
                    db.SaveChanges();

                }
                return Id;
            }
            catch (Exception ex)
            {
                throw new Exception("UpdateOrder: " + ex.Message);
            }

        }

        /// <summary>
        /// Consulta el estado de la orden
        /// </summary>
        /// <param name="RequestId"></param>
        /// <returns></returns>
        public string CheckPaymentStatus(int RequestId)
        {
            try
            {
                string status = string.Empty;
                PlacetoPay.Integrations.Library.CSharp.PlacetoPay P2P = new PlacetoPay.Integrations.Library.CSharp.PlacetoPay(PlaceToPay_Login, PlaceToPay_TranKey, new Uri(PlaceToPay_URL), Gateway.TP_REST);
                RedirectInformation response = P2P.Query(RequestId.ToString());

                if (response.IsApproved())
                    status = OrderStatus.PAYED;
                if (response.IsRejected())
                    status = OrderStatus.REJECTED;
                if (response.Status.status==OrderStatus.PENDING)
                    status = response.Status.status;

                return status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }


        /// <summary>
        /// Contacta a PlaceToPay para crear una instancia de pago
        /// </summary>
        /// <param name="order">Orden a pagar</param>
        /// <param name="info">Informacion adicional para la request</param>
        /// <returns></returns>
        private RedirectResponse GetRequestFromPlaceToPay(Order order, AdditionalOrderInfo info)
        {
            try
            {
                PlacetoPay.Integrations.Library.CSharp.PlacetoPay P2P = new PlacetoPay.Integrations.Library.CSharp.PlacetoPay(PlaceToPay_Login, PlaceToPay_TranKey, new Uri(PlaceToPay_URL), Gateway.TP_REST);
                Amount amount = new Amount(Convert.ToDouble(order.OrderCost),Currency);
                Payment payment = new Payment("OrderID_" + order.Id, "Compra TiendaEvertec", amount);
                RedirectRequest request = new RedirectRequest(payment,
                    URLToReturn + order.Id,
                    info.IP,
                    info.UserAgent,
                    DateTime.Now.AddMinutes(2).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"),
                     new Person(order.customer_document, order.customer_documenttype, order.customer_name, order.customer_surname, order.customer_email, null, order.customer_company, order.customer_mobile));

                RedirectResponse response = P2P.Request(request);
                return response;
            }
            catch (Exception ex)
            {

                throw new Exception("GetRequestFromPlaceToPay: " + ex.Message);
            }
           
        }

        /// <summary>
        ///  Devuelve Listado de Ordenes de la tienda
        /// </summary>
        /// <returns>Collection<Order></returns>
        public List<Order> GetOrderList()
        {
            try
            {
                List<Order> list = null;
                using (var db = new TiendaEvertecEntities())
                {
                    list = db.orders.ToList().Adapt<List<Order>>();
                }

                return list;

            }
            catch (Exception ex)
            {
                throw new Exception("GetOrderList(): "  + ex.Message);
            }
        }
         
    }
}
