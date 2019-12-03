using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Trading.DAO;

namespace Trading.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Trade()
        {
            ViewBag.Message = "Your Trade page.";

            Trading.BLL.Trade trade = new BLL.Trade();
            trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
            UploadTrade uploadTrade = trade.GetShippingTradeDetails(1);

            return View();
        }
    }
}