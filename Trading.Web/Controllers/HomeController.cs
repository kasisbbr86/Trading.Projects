using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Trading.DAO;
using Trading.Utilities;

namespace Trading.Web.Controllers
{
    public class HomeController : Controller
    {
        private TradeLogger logger = new TradeLogger();

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
            UploadTrade uploadTrade = new UploadTrade()
            {
                Shipping = new Shipping(),
                DocumentInstructions = new List<DocumentInstruction>(),
                ShippingModels = new List<ShippingModel>()
            };
            return View(uploadTrade);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            string path = Server.MapPath("~/TradeShippingSheets/");
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                string fname = string.Empty;
                HttpPostedFileBase file = files[i];
                // Checking for Internet Explorer  
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;
                }
                file.SaveAs(Path.Combine(path, fname));
            }
            return Json(files.Count + " Files Uploaded!");
        }

        [HttpPost]
        public ActionResult Trade(UploadTrade uploadTrade, HttpPostedFileBase postedFile, string submitButton)
        {
            try
            {
                switch (submitButton)
                {
                    case "Search":
                        ModelState.Clear();

                        Trading.BLL.Trade trade = new BLL.Trade();
                        trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
                        uploadTrade = trade.GetShippingTradeDetails(uploadTrade.ShippingId);
                        if (uploadTrade.Shipping.SINo == null || uploadTrade.Shipping.SINo == string.Empty) uploadTrade.IsSINoAvailable = false;
                        break;
                    //case "Upload":
                    //    if (postedFile != null)
                    //    {
                    //        string path = Server.MapPath("~/TradeShippingSheets/");
                    //        if (!Directory.Exists(path))
                    //        {
                    //            Directory.CreateDirectory(path);
                    //        }

                    //        postedFile.SaveAs(path + Path.GetFileName(postedFile.FileName));
                    //        uploadTrade = new UploadTrade()
                    //        {
                    //            Shipping = new Shipping(),
                    //            DocumentInstructions = new List<DocumentInstruction>(),
                    //            ShippingModels = new List<ShippingModel>()
                    //        };
                    //    }
                    //    break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return View(uploadTrade);
        }
    }
}