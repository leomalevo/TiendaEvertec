using PlacetoPay.Integrations.Library.CSharp.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TiendaEvertec.core.Business;
using TiendaEvertec.core.Entities;
using Mapster;
using System.Configuration;
using NLog;

namespace TiendaEvertec.Controllers
{
    public class CustomerController : Controller
    {

        #region Public Methods

       

        //Productos por defecto para orden
        public List<Product> listaProductos = new List<Product>();


        /// <summary>
        /// El constructor crea los productos de la orden (no era scope del test administrar los productos)
        /// </summary>
       public CustomerController()
        {
            listaProductos.Add(new Product() { ProductDescription = "Camiseta Deportiva L - DryFit", Quantity = 2, ValuePerUnit = 4000 });
            listaProductos.Add(new Product() { ProductDescription = "Pelota Nro 5 Adidas", Quantity = 1, ValuePerUnit = 5000 });
        }
       
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Action que muestra el form para crear la orden
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FillOrderForm()
        {
            return View(new Models.Order());
        }

        /// <summary>
        /// Action que guarda la Orden en la DB
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FillOrderForm(Models.Order order)
        {
            try
            {
                TiendaEvertec.core.Entities.Order newOrder = order.Adapt<TiendaEvertec.core.Entities.Order>();
                AdditionalOrderInfo additionalOrderInfo = new core.Entities.AdditionalOrderInfo(GetUserAgent(), GetIp());

                newOrder = OrdersManager.GetInstance().SaveOrder(newOrder, listaProductos);
                return RedirectToAction("PreviewOrder", new { Id=newOrder.Id });
            }
            catch (Exception ex)
            {
                logger.Error("FillOrderForm: " + ex.Message);
                throw new Exception("FillOrderForm: " + ex.Message);
            }


        }

        /// <summary>
        /// Action que hace un preview de la orden a pagar por el comprador
        /// </summary>
        /// <param name="Id">Id de la Orden</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PreviewOrder(int Id)
        {
            try
            {
                return View(OrdersManager.GetInstance().GetOrder(Id).Adapt<Models.Order>());
            }
            catch (Exception ex)
            {

                logger.Error("PreviewOrder: " + ex.Message);
                throw new Exception("PreviewOrder: " + ex.Message);
            }
            
        }

       /// <summary>
       /// Action que inicia la orden en el proceso de pago. Si todo sale ok, redirige a la web de PlacetoPay
       /// asi el comprador paga su orden
       /// </summary>
       /// <param name="Id">Id de la Orden a Pagar</param>
       /// <returns></returns>
        [HttpGet]
        public ActionResult ConfirmOrder(int Id)
        {
            try
            {
               
                AdditionalOrderInfo additionalOrderInfo = new core.Entities.AdditionalOrderInfo(GetUserAgent(), GetIp());
                //Se crea la request de pago
                RedirectResponse response = OrdersManager.GetInstance().CreatePaymentRequest(Id, additionalOrderInfo);

                //verifica el estado de la request
                ActionResult returnResult;
                if (response.Status.status == "OK")
                {
                    //debe reenviar al WebCheckout de PlacetoPay
                    returnResult = Redirect(response.ProcessUrl);
                }
                else
                {
                    ViewBag.ErrorMessage = response.Status.Message;
                    returnResult = View("OrderSaveError");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                logger.Error("Buy: " + ex.Message);
                throw new Exception("Buy: " + ex.Message);
            }


        }



        /// <summary>
        /// Action que viene desde PlaceToPay para verificar el estado de la Orden. 
        /// Si el pago fue ok, le notifica al comprador que inicia el despacho de su compra
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OrderSaved(int Id)
        {
            try
            {
                TiendaEvertec.core.Entities.Order order = OrdersManager.GetInstance().GetOrder(Id);
                string status = OrdersManager.GetInstance().CheckPaymentStatus((int)order.IdRequest);
                OrdersManager.GetInstance().UpdateOrder(Id, status);

                if (status == OrderStatus.REJECTED || status == OrderStatus.PENDING)
                    return RedirectToAction("OrderPendingOrRejected", new { Id = Id });

                return View(OrdersManager.GetInstance().GetOrder(Id).Adapt<Models.Order>());
            }
            catch (Exception ex)
            {
                logger.Error("OrderSaved: " + ex.Message);
                throw new Exception("OrderSaved: " + ex.Message);
            }
           
        }


        /// <summary>
        /// Action que devuelve la orden para consultar
        /// </summary>
        /// <param name="Id">Id de la orden</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewOrder(int Id)
        {
            try
            {
                TiendaEvertec.core.Entities.Order order = OrdersManager.GetInstance().GetOrder(Id);
                Models.Order newOrder = order.Adapt<Models.Order>();
                return View(newOrder);
            }
            catch (Exception ex)
            {
                logger.Error("ViewOrder: " + ex.Message);
                throw new Exception("ViewOrder: " + ex.Message);
            }
           
        }


        /// <summary>
        /// Action que muestra un mensaje de fallo en la compra (por un error en el pago) y le consulta al comprador
        /// si desea re intentar el pago. 
        /// </summary>
        /// <param name="Id">Id de la Orden</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OrderPendingOrRejected(int Id)
        {
            try
            {
                Models.Order newOrder = OrdersManager.GetInstance().GetOrder(Id).Adapt<Models.Order>();
                ViewBag.StatusInSpanish = newOrder.status == "REJECTED" ? "Rechazada" : (newOrder.status == "PENDING" ? "Pendiente" : newOrder.status);
                return View(newOrder);
            }
            catch (Exception ex)
            {
                logger.Error("OrderPendingOrRejected: " + ex.Message);
                throw new Exception("OrderPendingOrRejected: " + ex.Message);
            }

        }



        /// <summary>
        /// Action que inicia una nueva orden en respuesta a la fallo de una orden previa (por algun problema con el pago)
        /// </summary>
        /// <param name="Id">Id de la Orden a re-enviar</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ReSendOrder(int Id)
        {
            try
            {
                TiendaEvertec.core.Entities.Order previuosOrder=OrdersManager.GetInstance().GetOrder(Id);
                TiendaEvertec.core.Entities.Order newOrder = previuosOrder.Adapt<TiendaEvertec.core.Entities.Order>();
                //setea los estados nuevos ya que es una copia de la orden rechazada
                newOrder.Id = 0;
                newOrder.updated_at = null;


                AdditionalOrderInfo additionalOrderInfo = new core.Entities.AdditionalOrderInfo(GetUserAgent(), GetIp());

                //guarda la nueva orden y genero una nueva request de pago
                newOrder = OrdersManager.GetInstance().SaveOrder(newOrder, listaProductos);
                RedirectResponse response = OrdersManager.GetInstance().CreatePaymentRequest(newOrder.Id, additionalOrderInfo);

                //chequea como fue la creacion e inicia el CheckOut
                ActionResult returnResult;
                if (response.Status.status == "OK")
                {
                    //debe reenviar al WebCheckout de PlacetoPay
                    returnResult = Redirect(response.ProcessUrl);
                }
                else
                {
                    ViewBag.ErrorMessage = response.Status.Message;
                    returnResult = View("OrderSaveError");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                logger.Error("Buy: " + ex.Message);
                throw new Exception("Buy: " + ex.Message);
            }


        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Metodo que devuelve la IP del cliente
        /// </summary>
        /// <returns></returns>
        private string GetIp()
        {
            try
            {
                string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }


                return ip;
            }
            catch (Exception ex)
            {

                throw new Exception("GetIP: " + ex.Message);
            }

        }
        /// <summary>
        /// Metodo que devuelve el sistema que consume el servicio
        /// </summary>
        /// <returns></returns>
        private string GetUserAgent()
        {
            try
            {
                return Request.Headers["User-Agent"].ToString();
            }
            catch (Exception ex)
            {

                throw new Exception("GetUserAgent: " + ex.Message);
            }

        }


        #endregion

    }
}
