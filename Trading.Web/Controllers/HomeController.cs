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

            UploadTrade uploadTrade = new UploadTrade() { 
                Shipping = new Shipping(),
                DocumentInstructions = new List<DocumentInstruction>(),
                ShippingModels = new List<ShippingModel>()
            };

            return View(uploadTrade);
        }

        [HttpPost]
        public ActionResult Trade(UploadTrade uploadTrade)
        {
            ViewBag.Message = "Your Trade page.";
            ModelState.Clear();

            Trading.BLL.Trade trade = new BLL.Trade();
            trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
            uploadTrade = trade.GetShippingTradeDetails(uploadTrade.ShippingId);

            return View(uploadTrade);
        }
    }
}