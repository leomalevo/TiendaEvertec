using Mapster;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TiendaEvertec.core.Business;
using TiendaEvertec.core.Data;
using TiendaEvertec.Models;

namespace TiendaEvertec.Controllers
{
    public class AdminController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public string Currency = ConfigurationManager.AppSettings["Currency"];
        public string CurrencySymbol = ConfigurationManager.AppSettings["CurrencySymbol"];

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult OrderList()
        {
            try
            {
                ViewBag.CurrencySymbol = CurrencySymbol;
                ViewBag.OrdersList = ((List<Models.Order>)OrdersManager.GetInstance().GetOrderList().Adapt<List<Models.Order>>()).OrderByDescending(x=>x.Id);
                return View();
            }
            catch (Exception ex)
            {
                logger.Error("OrderList: " + ex.Message);
                throw new Exception("OrderList: " + ex.Message);
            }
           
        }
      
    }
}
