using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Trading.BLL;
using Trading.DAO;
using Trading.ViewModel;

namespace Trading.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        //public ActionResult SAUpload()
        //{
        //    return View();
        //}

        public ActionResult AddNewDocument(int masterID)
        {
            IncomingCourierMasterVM incomingCourierMasterVM = GetIncomingCourierandDetails(masterID);
            return View(incomingCourierMasterVM);
        }

        [HttpPost]
        public ActionResult AddNewCourierMaster(IncomingCourierMasterVMSave courierMaster)
        {
            Guid companyId = Guid.NewGuid();
            courierMaster.CompanyID = companyId;
            courierMaster.CreationDate = courierMaster.UpdatedDate = DateTime.Now;
            courierMaster.CreatedBy = courierMaster.UpdatedBy = 999;

            Trading.BLL.Trade trade = new BLL.Trade();
            trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
            int masterId = trade.SaveIncomingCourierMaster(courierMaster);
            courierMaster.MasterID = masterId;
            return Json(courierMaster);
        }

        [HttpPost]
        public ActionResult AddNewDocument(IncomingCourierMasterVM master, List<IncomingCourierDetailsVM> detailRows)
        {
            Trading.BLL.Trade trade = new BLL.Trade();
            trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
            List<IncomingCourierDetailsVMSave> incomingCourierDetailsList = new List<IncomingCourierDetailsVMSave>();
            int parentDetailID = 0;
            detailRows.Where(detail => detail.IsSubDetail == false).ToList().ForEach(element => {
                IncomingCourierDetailsVMSave saveElement = element;
                parentDetailID = element.DetailsID;
                saveElement.DetailsID = element.DetailsID;
                saveElement.MasterID = master.MasterID;
                saveElement.CompanyID = master.CompanyID;
                incomingCourierDetailsList.Add(saveElement);
            });
            detailRows.Where(detail => detail.IsSubDetail == true).ToList().ForEach(element => {
                IncomingCourierDetailsVMSave saveElement = element;
                saveElement.DetailsID = element.DetailsID;
                saveElement.MasterID = master.MasterID;
                saveElement.CompanyID = master.CompanyID;
                saveElement.ParentDetailsID = parentDetailID;
                incomingCourierDetailsList.Add(saveElement);
            });
            int masterId = trade.SaveIncomingCourierDetails(incomingCourierDetailsList);
            return Json(detailRows);
        }

        public IncomingCourierMasterVM GetCourierMaster(FormCollection fc, Guid companyId)
        {
            IncomingCourierMasterVM incomingCourierMasterVM = new IncomingCourierMasterVM();
            incomingCourierMasterVM.CompanyID = companyId;
            incomingCourierMasterVM.AWBNo = (fc["AwbNo"]);
            incomingCourierMasterVM.CourierCompany = Convert.ToInt32(fc["CourierCompany"]);
            incomingCourierMasterVM.CourierFor = Convert.ToInt32(fc["CourierFor"]);
            incomingCourierMasterVM.DocumentType = Convert.ToInt32(fc["DocumentType"]);
            incomingCourierMasterVM.ReceivedFrom = Convert.ToInt32(fc["ReceivedFrom"]);
            incomingCourierMasterVM.HandedOverOn = DateTime.Parse(fc["HandedOverOn"]);
            incomingCourierMasterVM.ReceivedOn = DateTime.Parse(fc["ReceivedOn"]);
            incomingCourierMasterVM.CreatedBy = 12;
            incomingCourierMasterVM.CreationDate = DateTime.Now;
            incomingCourierMasterVM.UpdatedBy = 12;
            incomingCourierMasterVM.UpdatedDate = DateTime.Now;
            return incomingCourierMasterVM;
        }

        public List<IncomingCourierDetailsVM> GetCourierDetails(FormCollection fc, Guid companyId)
        {
            List<IncomingCourierDetailsVM> incomingCourierDetailsVMLst = new List<IncomingCourierDetailsVM>();
            int rowId = 0;
            for (int f = 0; f < fc.Count; f++)
            {
                string currentKey = fc.GetKey(f);
                if (currentKey.Contains("Date_" + rowId))
                {
                    IncomingCourierDetailsVM incomingCourierDetailsVM = new IncomingCourierDetailsVM();
                    incomingCourierDetailsVM.CompanyID = companyId;
                    incomingCourierDetailsVM.Date = DateTime.Parse(fc["Date_" + rowId]);
                    incomingCourierDetailsVM.SINo = (fc["SINo_" + rowId]);
                    incomingCourierDetailsVM.Remarks = fc["Remarks_" + rowId];
                    incomingCourierDetailsVM.DocDetail = fc["DocDetail_" + rowId];
                    incomingCourierDetailsVM.FileName = (fc["FileName_" + rowId]);
                    incomingCourierDetailsVM.FilePath = (fc["FilePath_" + rowId]);
                    incomingCourierDetailsVM.MasterID = Convert.ToInt32((fc["MasterID_" + rowId]));
                    incomingCourierDetailsVM.Qty = Convert.ToDecimal(fc["Qty_" + rowId]);
                    incomingCourierDetailsVM.RefNo = (fc["RefNo_" + rowId]);
                    incomingCourierDetailsVM.ArraySubItem = fc["arraySubItem_" + rowId];
                    incomingCourierDetailsVM.IsSubDetail = false;
                    incomingCourierDetailsVM.ParentDetailsID = 0;
                    incomingCourierDetailsVM.CreatedBy = 12;
                    incomingCourierDetailsVM.CreationDate = DateTime.Now;
                    incomingCourierDetailsVM.UpdatedBy = 12;
                    incomingCourierDetailsVM.UpdatedDate = DateTime.Now;
                    incomingCourierDetailsVM.SubDetails = GetSubDetails(incomingCourierDetailsVM);
                    incomingCourierDetailsVMLst.Add(incomingCourierDetailsVM);
                }
                rowId++;
            }
            return incomingCourierDetailsVMLst;
        }

        public List<IncomingCourierSubDetailsVM> GetSubDetails(IncomingCourierDetailsVM incomingDetails)
        {
            List<IncomingCourierSubDetailsVM> subDetailsLst = new List<IncomingCourierSubDetailsVM>();
            string subItem = incomingDetails.ArraySubItem;
            subItem = subItem.Replace("$$Falcon$$", "#");
            string[] row = subItem.Split('#');
            string xrow = row[1];
            if (xrow.EndsWith("|"))
                xrow = xrow.Substring(0, xrow.Length - 1);

            string[] yrow = xrow.Split('|');
            for (var x = 0; x < yrow.Length; x++)
            {
                IncomingCourierSubDetailsVM subItems = new IncomingCourierSubDetailsVM();
                string[] batchrecords = Regex.Split(yrow[x], "&&");
                if (Convert.ToDecimal(batchrecords[1]) != 0)
                {
                    subItems.CompanyID = incomingDetails.CompanyID;
                    subItems.Date = incomingDetails.Date;
                    subItems.SINo = incomingDetails.SINo;
                    subItems.Remarks = batchrecords[2];
                    subItems.DocDetail = batchrecords[2];
                    subItems.FileName = batchrecords[2];
                    subItems.FilePath = batchrecords[2];
                    subItems.MasterID = incomingDetails.MasterID;
                    subItems.Qty = Convert.ToDecimal(batchrecords[2]);
                    subItems.RefNo = batchrecords[2];
                    subItems.ArraySubItem = "";
                    subItems.IsSubDetail = true;
                    subItems.ParentDetailsID = 0;
                    subItems.CreatedBy = 12;
                    subItems.CreationDate = DateTime.Now;
                    subItems.UpdatedBy = 12;
                    subItems.UpdatedDate = DateTime.Now;
                    subDetailsLst.Add(subItems);
                }
            }
            return subDetailsLst;
        }

        public IncomingCourierMasterVM GetIncomingCourierandDetails(int masterID)
        {
            Trading.BLL.Trade trade = new BLL.Trade();
            trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
            IncomingCourierMasterVM incomingCourierMasterVM = trade.GetCourierMasterDetails(masterID);
            List<IncomingCourierDetailsVM> incomingDetails = new List<IncomingCourierDetailsVM>();
            if (incomingCourierMasterVM.CourierDetails.Count == 0)
            {
                incomingCourierMasterVM.CourierDetails.AddRange(new List<IncomingCourierDetailsVM>() {
                        new IncomingCourierDetailsVM() { DocDetail = "B/L-Bill of Leading", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "C/0-Country of origin", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "v/C-Vessel Certficate", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "P/L-Packing List", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "Verning List", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "Serail Number List", IsSubDetail = true }
                    }
                );
            }
            if (incomingCourierMasterVM != null)
            {
                incomingDetails = incomingCourierMasterVM.CourierDetails.ToList();
                var getMainItemDetails = incomingDetails.Where(x => x.IsSubDetail == false);
                incomingCourierMasterVM.CourierDetails.Clear();
                foreach (var details in getMainItemDetails)
                {
                    details.ArraySubItem = CompressSubDetails(incomingDetails, details.DetailsID);
                    incomingCourierMasterVM.CourierDetails.Add(details);
                }
            }

            return incomingCourierMasterVM;
        }

        public JsonResult GetJsonIncomingCourierandDetails(int masterID)
        {
            Trading.BLL.Trade trade = new BLL.Trade();
            trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
            IncomingCourierMasterVM incomingCourierMasterVM = trade.GetCourierMasterDetails(masterID);
            List<IncomingCourierDetailsVM> incomingDetails = new List<IncomingCourierDetailsVM>();
            if (incomingCourierMasterVM.CourierDetails.Count == 0)
            {
                incomingCourierMasterVM.CourierDetails.AddRange(new List<IncomingCourierDetailsVM>() {
                        new IncomingCourierDetailsVM() { DocDetail = "B/L-Bill of Leading", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "C/0-Country of origin", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "v/C-Vessel Certficate", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "P/L-Packing List", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "Verning List", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "Serail Number List", IsSubDetail = true }
                    }
                );
            }
            if (incomingCourierMasterVM != null)
            {
                incomingDetails = incomingCourierMasterVM.CourierDetails.ToList();
                var getMainItemDetails = incomingDetails.Where(x => x.IsSubDetail == false);
                incomingCourierMasterVM.CourierDetails.Clear();
                foreach (var details in getMainItemDetails)
                {
                    details.ArraySubItem = CompressSubDetails(incomingDetails, details.DetailsID);
                    incomingCourierMasterVM.CourierDetails.Add(details);
                }
            }
            return Json(incomingCourierMasterVM, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCourierDetailsRecord(string parentDetailID)
        {
            List<IncomingCourierDetailsVM> result = new List<IncomingCourierDetailsVM>();
            Trading.BLL.Trade trade = new BLL.Trade();
            trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
            parentDetailID = parentDetailID.IndexOf("_") > 0 ? parentDetailID.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[1] : parentDetailID;
            IncomingCourierMasterVM incomingCourierMasterVM = trade.GetCourierDocumentDetails(parentDetailID == ""? 0 : Convert.ToInt32(parentDetailID));
            List<IncomingCourierDetailsVM> incomingDetails = new List<IncomingCourierDetailsVM>();
            if (parentDetailID == "" || incomingCourierMasterVM.CourierDetails.Count == 0)
            {
                incomingCourierMasterVM.CourierDetails.AddRange(new List<IncomingCourierDetailsVM>() {
                        new IncomingCourierDetailsVM() { DocDetail = "B/L-Bill of Leading", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "C/0-Country of origin", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "v/C-Vessel Certficate", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "P/L-Packing List", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "Verning List", IsSubDetail = true },
                        new IncomingCourierDetailsVM() { DocDetail = "Serail Number List", IsSubDetail = true }
                    }
                );
            }
            if (incomingCourierMasterVM != null)
            {
                incomingDetails = incomingCourierMasterVM.CourierDetails.ToList();
                var getMainItemDetails = incomingDetails.Where(x => x.IsSubDetail == false); //&& x.SINo == masterID
                incomingCourierMasterVM.CourierDetails.Clear();
                foreach (var details in incomingDetails.Where(x => x.IsSubDetail == true))
                {
                    details.ArraySubItem = CompressSubDetails(incomingDetails, details.DetailsID);
                    incomingCourierMasterVM.CourierDetails.Add(details);
                    result.Add(details);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public string CompressSubDetails(List<IncomingCourierDetailsVM> incomingCourierDetails, Int64 detailsID)
        {
            List<IncomingCourierDetailsVM> getSubItems = incomingCourierDetails.Where(x => x.ParentDetailsID == detailsID).ToList();
            string Falcon = "";
            Falcon += "$$Falcon$$";
            foreach (var details in getSubItems)
            {
                Falcon += details.DocDetail + "&&" + details.RefNo + "&&" + details.Qty + "&&" + details.Remarks + "&&" + details.FileName + "&&" +
                        details.FilePath + "&&" + details.DetailsID + "&&" + details.ParentDetailsID + "|";
            }
            Falcon += "$$Falcon$$";
            return Falcon;
        }
        public ActionResult CourierList()
        {
            List<Courier> courierList = new List<Courier>();
            //get data
            Trading.BLL.Trade trade = new BLL.Trade();
            trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
            courierList = trade.GetCourierList();

            return View(courierList);
        }

        public ActionResult SIList()
        {
            List<ViewModel.Shipping> shippingModel = GetDetailsFromShipping();
            return View(shippingModel);
        }
        public List<ViewModel.Shipping> GetDetailsFromShipping()
        {
            Trading.BLL.Trade trade = new BLL.Trade();
            trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
            List<ViewModel.Shipping> shippingModel = trade.GetShippingDetails();
            return shippingModel;
        }

        [HttpPost]
        public ActionResult SIList(HttpPostedFileBase postedFile)
        {
            UploadTradeVM uploadTradeVM = new UploadTradeVM();
            List<ViewModel.Shipping> shippingModel = new List<ViewModel.Shipping>();
            try
            {
                if (postedFile != null)
                {
                    string path = Server.MapPath("~/TradeShippingSheets/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    postedFile.SaveAs(path + Path.GetFileName(postedFile.FileName));
                    uploadTradeVM = new UploadTradeVM()
                    {
                        Shipping = new ViewModel.Shipping(),
                        DocumentInstructions = new List<DocumentInstructionVM>(),
                        ShippingModels = new List<ShippingModelVM>()
                    };

                    ProcessInputFile((path + Path.GetFileName(postedFile.FileName)));

                    shippingModel = GetDetailsFromShipping();
                    ViewBag.Exception = "File uploaded successfully";
                }
                return View(shippingModel);
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex.Message;
                shippingModel = GetDetailsFromShipping();
                return View(shippingModel);
            }
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

        public ActionResult Trade(string shippingId)
        {
            int cnvrtShippingID = Convert.ToInt32(shippingId);
            ViewBag.Message = "Your Trade page.";
            Trading.BLL.Trade trade = new BLL.Trade();
            trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
            UploadTradeVM uploadTrade = trade.GetShippingTradeDetails(cnvrtShippingID);
            uploadTrade = ReEvaluateUploadTrade(uploadTrade);
            return View(uploadTrade);
        }

        // SA Upload 

        public ActionResult SAUpload()
        {
            List<ViewModel.ShippingAdviceVM> shippingAdviceModel = GetDetailsFromShippingAdvice();
            return View(shippingAdviceModel);
        }

        public List<ViewModel.ShippingAdviceVM> GetDetailsFromShippingAdvice()
        {
            Trading.BLL.Trade trade = new BLL.Trade();
            trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
            List<ViewModel.ShippingAdviceVM> shippingAdviceModel = trade.GetShippingAdvice();
            return shippingAdviceModel;
        }

        [HttpPost]
        public ActionResult SAUpload( HttpPostedFileBase SApostedFile)
        {
            ShippingAdviceVM shippingAdviceVM = new ShippingAdviceVM();
            List<ViewModel.ShippingAdviceVM> shippingAdviceModel = new List<ViewModel.ShippingAdviceVM>();
            try
            {
                if (SApostedFile != null)
                {
                    string path = Server.MapPath("~/ShippingAdviseSheets/");
                    if (Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    SApostedFile.SaveAs(path + Path.GetFileName(SApostedFile.FileName));
                    shippingAdviceVM = new ShippingAdviceVM();
                    ProessSASheet((path + Path.GetFileName(SApostedFile.FileName)));

                    shippingAdviceModel = GetDetailsFromShippingAdvice();
                    
                }
                return View(shippingAdviceModel);
            }
            catch (Exception ex)
            {
                //throw ex;
                TempData["SuccessMessage"] = ex.Message;
                shippingAdviceModel = GetDetailsFromShippingAdvice();
                return View(shippingAdviceModel);

            }
            
        }

        [HttpPost]
        public ActionResult Trade(UploadTrade uploadTrade, HttpPostedFileBase postedFile, string submitButton)
        {
            UploadTradeVM uploadTradeVM = new UploadTradeVM();
            try
            {
                switch (submitButton)
                {
                    case "Search":
                        ModelState.Clear();

                        Trading.BLL.Trade trade = new BLL.Trade();
                        trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
                        uploadTradeVM = trade.GetShippingTradeDetails(uploadTrade.ShippingId);
                        uploadTradeVM = ReEvaluateUploadTrade(uploadTradeVM);
                        if (uploadTrade.Shipping.SINo == null || uploadTrade.Shipping.SINo == string.Empty) uploadTrade.IsSINoAvailable = false;
                        break;
                    case "Upload":
                        if (postedFile != null)
                        {
                            string path = Server.MapPath("~/TradeShippingSheets/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            postedFile.SaveAs(path + Path.GetFileName(postedFile.FileName));
                            uploadTradeVM = new UploadTradeVM()
                            {
                                Shipping = new ViewModel.Shipping(),
                                DocumentInstructions = new List<DocumentInstructionVM>(),
                                ShippingModels = new List<ShippingModelVM>()
                            };

                            ProcessInputFile((path + Path.GetFileName(postedFile.FileName)));
                        }
                        break;
                }
                return View(uploadTradeVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UploadTradeVM ReEvaluateUploadTrade(UploadTradeVM uploadTrade)
        {
            List<DocumentInstructionVM> lstDocumentInstructions = new List<DocumentInstructionVM>();
            List<ShippingModelVM> lstShippingModels = new List<ShippingModelVM>();
            DocumentInstructionVM documentInstructionVM;
            ShippingModelVM shippingModelVM;
            var documentList = uploadTrade.DocumentInstructions.Select(x => x.Instruction.Split(new string[] { "AND" }, StringSplitOptions.RemoveEmptyEntries)).ToList();
            foreach (var it in documentList)
            {
                documentInstructionVM = new DocumentInstructionVM();
                documentInstructionVM.Instruction = Regex.Replace(it[0], "[^A-Za-z ]", "");
                //string clean = Regex.Replace(it[0], "[^A-Za-z ]", ""); ;
                var resultString = Regex.Match(it[1], @"\d+").Value;
                if (resultString != "")
                    documentInstructionVM.Copies = Int32.Parse(resultString);
                else
                    documentInstructionVM.Copies = 0;
                lstDocumentInstructions.Add(documentInstructionVM);
            }
            uploadTrade.DocumentInstructions.Clear();
            uploadTrade.DocumentInstructions = lstDocumentInstructions;

            foreach (var it in uploadTrade.ShippingModels)
            {
                shippingModelVM = new ShippingModelVM();
                shippingModelVM.BLModelName = it.BLModelName;
                shippingModelVM.Description = it.Description;
                shippingModelVM.LineNo = it.PONo.Substring(18, 4);
                shippingModelVM.PONo = it.PONo.Substring(10, 7);
                shippingModelVM.ModelName = it.ModelName;
                shippingModelVM.Quantity = it.Quantity;
                //shippingModelVM.SCNo = it.Remarks.Substring(10, 5);
                if (it.Remarks.Length > 0)
                    shippingModelVM.SCNo = it.Remarks.Substring(it.Remarks.Length - 8).Remove(7);
                else
                    shippingModelVM.SCNo = it.Remarks;
                shippingModelVM.Version = it.Version;
                lstShippingModels.Add(shippingModelVM);
            }
            uploadTrade.ShippingModels.Clear();
            uploadTrade.ShippingModels = lstShippingModels;
            return uploadTrade;

        }

        public void ProessSASheet(string fileFullPath)
        {
            try
            {

                Trading.BLL.Trade trade = new BLL.Trade();
                trade.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;

                List<ShippingAdviceVM> currentMothTimeSheetList = new List<ShippingAdviceVM>();
                FileStream currentTimeSheetFile = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);

                XSSFWorkbook hssfwb = new XSSFWorkbook(currentTimeSheetFile);
                XSSFFormulaEvaluator formula = new XSSFFormulaEvaluator(hssfwb);

                ISheet WorkerTimeSheet = hssfwb.GetSheet("mike");
                ICellStyle style1 = hssfwb.CreateCellStyle();
                IDataFormat dataFormatCustom = hssfwb.CreateDataFormat();

                style1.DataFormat = dataFormatCustom.GetFormat("dd/MM/yyyy HH:mm:ss");

                Guid companyID = Guid.Parse("CCAA3E3F-4486-4465-B5A1-723F647EAD17");
                //DateTime TimeSheetDateValue = WorkerTimeSheet.GetRow(2).GetCell(3).DateCellValue;
                //int days = DateTime.DaysInMonth(TimeSheetDateValue.Year, TimeSheetDateValue.Month);
                for (int row = 5; row < WorkerTimeSheet.LastRowNum-1; row++) //Loop the records upto filled row  
                {
                    //System.IO.File.AppendAllText(log, "Reading " + row + " row For EMPCode  " + getValueFromCell(WorkerTimeSheet.GetRow(row).GetCell(1), formula));
                    ShippingAdviceVM shippingAdviceVM = new ShippingAdviceVM();
                    var currentCellValue2 = WorkerTimeSheet.GetRow(row);
                   
                    // string currentCellValue = getValueFromCell(WorkerTimeSheet.GetRow(row).GetCell(2), formula);
                    //if (currentCellValue != "" && currentCellValue.ToLower().Equals("total"))
                    //{
                    //    break;
                    //}
                    if (currentCellValue2 == null)
                        continue;
                    string currentCellValue = WorkerTimeSheet.GetRow(row).GetCell(2).StringCellValue;
                    if (currentCellValue != null && currentCellValue != "") //null is when the row only contains empty cells   
                    {
                        //int validRow = Regex.Matches(currentCellValue.Substring(0, 1), @"[a-zA-Z]").Count;
                        //if (validRow == 0)
                        //{
                        shippingAdviceVM.CompanyID = companyID;

                        var valueFrom2thCell = WorkerTimeSheet.GetRow(row).GetCell(2);
                        if (valueFrom2thCell == null)
                            shippingAdviceVM.SCInvoiceNo = " ";
                        else
                            shippingAdviceVM.SCInvoiceNo = WorkerTimeSheet.GetRow(row).GetCell(2).StringCellValue;

                        var valueFrom3thCell = WorkerTimeSheet.GetRow(row).GetCell(3);
                        if (valueFrom3thCell == null)
                            shippingAdviceVM.InvoiceAmount = 0;
                        else
                            shippingAdviceVM.InvoiceAmount = Convert.ToDecimal(WorkerTimeSheet.GetRow(row).GetCell(3).NumericCellValue);

                        var valueFrom4thCell = WorkerTimeSheet.GetRow(row).GetCell(4);
                        if (valueFrom4thCell == null)
                            shippingAdviceVM.Consignee = string.Empty;
                        else if (valueFrom4thCell.CellType == CellType.Numeric)
                            shippingAdviceVM.Consignee = Convert.ToString(WorkerTimeSheet.GetRow(row).GetCell(4).NumericCellValue);
                        else
                            shippingAdviceVM.Consignee = WorkerTimeSheet.GetRow(row).GetCell(4).StringCellValue;

                        var valueFrom5thCell = WorkerTimeSheet.GetRow(row).GetCell(5);
                        if (valueFrom5thCell == null)
                            shippingAdviceVM.BLDate = DateTime.Now;
                        else
                            shippingAdviceVM.BLDate = DateTime.Parse(WorkerTimeSheet.GetRow(row).GetCell(5).StringCellValue);

                        var valueFrom6thCell = WorkerTimeSheet.GetRow(row).GetCell(6);
                        if (valueFrom6thCell == null)
                            shippingAdviceVM.ReceivedDate = DateTime.Now; 
                        else
                            shippingAdviceVM.ReceivedDate = DateTime.Parse(WorkerTimeSheet.GetRow(row).GetCell(6).StringCellValue);

                        var valueFrom7thCell = WorkerTimeSheet.GetRow(row).GetCell(7);
                        if (valueFrom7thCell == null)
                            shippingAdviceVM.Shiper = string.Empty;
                        else
                            shippingAdviceVM.Shiper = WorkerTimeSheet.GetRow(row).GetCell(7).StringCellValue;

                        var valueFrom8thCell = WorkerTimeSheet.GetRow(row).GetCell(8);
                        if (valueFrom8thCell == null)
                            shippingAdviceVM.BLNo = string.Empty;
                        else if (valueFrom8thCell.CellType == CellType.Numeric)
                            shippingAdviceVM.Consignee = Convert.ToString(WorkerTimeSheet.GetRow(row).GetCell(4).NumericCellValue);
                        else
                            shippingAdviceVM.BLNo = WorkerTimeSheet.GetRow(row).GetCell(8).StringCellValue;

                        var valueFrom9thCell = WorkerTimeSheet.GetRow(row).GetCell(9);
                        if (valueFrom9thCell == null)
                            shippingAdviceVM.Factory = " ";
                        else
                            shippingAdviceVM.Factory = WorkerTimeSheet.GetRow(row).GetCell(9).StringCellValue;

                        var valueFrom10thCell = WorkerTimeSheet.GetRow(row).GetCell(10);
                        if (valueFrom10thCell == null)
                            shippingAdviceVM.Department = string.Empty;
                        else
                            shippingAdviceVM.Department = WorkerTimeSheet.GetRow(row).GetCell(10).StringCellValue;

                        var valueFrom11thCell = WorkerTimeSheet.GetRow(row).GetCell(11);
                        if (valueFrom11thCell == null)
                            shippingAdviceVM.Material = string.Empty;
                        else
                            shippingAdviceVM.Material = WorkerTimeSheet.GetRow(row).GetCell(11).StringCellValue;
                    
                        var valueFrom12thcell = WorkerTimeSheet.GetRow(row).GetCell(12);
                        if (valueFrom12thcell == null)
                            shippingAdviceVM.Quantity = 0;
                       else if (valueFrom12thcell.CellType == CellType.Numeric)
                            shippingAdviceVM.Quantity = Convert.ToInt32(WorkerTimeSheet.GetRow(row).GetCell(12).NumericCellValue);
                        else
                            shippingAdviceVM.Quantity = Convert.ToInt32(WorkerTimeSheet.GetRow(row).GetCell(12).NumericCellValue);

                        var valueFrom13thCell = WorkerTimeSheet.GetRow(row).GetCell(13);
                        if (valueFrom13thCell == null)
                            shippingAdviceVM.FOB = 0;
                        else
                            shippingAdviceVM.FOB = Convert.ToDecimal(WorkerTimeSheet.GetRow(row).GetCell(13).NumericCellValue);

                        var valueFrom14thCell = WorkerTimeSheet.GetRow(row).GetCell(14);
                        if (valueFrom14thCell == null)
                            shippingAdviceVM.PurchaseDocumentNo = 0;
                        else
                            shippingAdviceVM.PurchaseDocumentNo = Convert.ToInt32(WorkerTimeSheet.GetRow(row).GetCell(14).NumericCellValue);

                        var valueFrom15thCell = WorkerTimeSheet.GetRow(row).GetCell(15);
                        if (valueFrom15thCell == null)
                            shippingAdviceVM.Item1 = 0;
                        else
                            shippingAdviceVM.Item1 = Convert.ToInt32(WorkerTimeSheet.GetRow(row).GetCell(15).NumericCellValue);

                        var valueFrom16thcell = WorkerTimeSheet.GetRow(row).GetCell(16);
                        if (valueFrom16thcell == null)
                            shippingAdviceVM.SAPSO = string.Empty;
                        else
                            shippingAdviceVM.SAPSO = Convert.ToString(WorkerTimeSheet.GetRow(row).GetCell(16).NumericCellValue);

                        var valueFrom17thCell = WorkerTimeSheet.GetRow(row).GetCell(17);
                        if (valueFrom17thCell == null)
                            shippingAdviceVM.Item2 = 0;
                        else
                            shippingAdviceVM.Item2 = Convert.ToInt32(WorkerTimeSheet.GetRow(row).GetCell(17).NumericCellValue);

                        var valueFrom18thcell = WorkerTimeSheet.GetRow(row).GetCell(16);
                        if (valueFrom18thcell == null)
                            shippingAdviceVM.SAPDO = string.Empty;
                        else
                            shippingAdviceVM.SAPDO = Convert.ToString(WorkerTimeSheet.GetRow(row).GetCell(18).NumericCellValue);

                        var valueFrom19thCell = WorkerTimeSheet.GetRow(row).GetCell(19);
                        if (valueFrom19thCell == null)
                            shippingAdviceVM.PInt = 0;
                        else
                            shippingAdviceVM.PInt = Convert.ToInt32(WorkerTimeSheet.GetRow(row).GetCell(19).NumericCellValue);

                        var valueFrom20thcell = WorkerTimeSheet.GetRow(row).GetCell(20);
                        if (valueFrom20thcell == null)
                            shippingAdviceVM.SLoc = string.Empty;
                        else if (valueFrom20thcell.CellType == CellType.Numeric)
                            shippingAdviceVM.SLoc = Convert.ToString(WorkerTimeSheet.GetRow(row).GetCell(20).NumericCellValue);
                        else
                            shippingAdviceVM.SLoc = WorkerTimeSheet.GetRow(row).GetCell(20).StringCellValue;

                        var valueFrom21stcell = WorkerTimeSheet.GetRow(row).GetCell(21);
                        if (valueFrom21stcell == null)
                            shippingAdviceVM.Temp1 = string.Empty;
                        else
                            shippingAdviceVM.Temp1 = WorkerTimeSheet.GetRow(row).GetCell(21).StringCellValue;

                        var valueFrom22thCell = WorkerTimeSheet.GetRow(row).GetCell(22);
                        if (valueFrom22thCell == null)
                            shippingAdviceVM.Seq = 0;
                        else
                            shippingAdviceVM.Seq = Convert.ToInt32(WorkerTimeSheet.GetRow(row).GetCell(22).NumericCellValue);

                        var valueFrom23rdcell = WorkerTimeSheet.GetRow(row).GetCell(23);
                        if (valueFrom23rdcell == null)
                            shippingAdviceVM.Del = string.Empty;
                        else
                            shippingAdviceVM.Del = WorkerTimeSheet.GetRow(row).GetCell(23).StringCellValue;

                        var valueFrom24thCell = WorkerTimeSheet.GetRow(row).GetCell(24);
                        if (valueFrom24thCell == null)
                            shippingAdviceVM.Comp = string.Empty;
                        else
                            shippingAdviceVM.Comp = WorkerTimeSheet.GetRow(row).GetCell(24).StringCellValue;

                        //var valueFrom25thcell = WorkerTimeSheet.GetRow(row).GetCell(25);
                        //if (valueFrom25thcell == null)
                        //    shippingAdviceVM.DeliveryDate = DateTime.Now;
                        //else
                        var sd = WorkerTimeSheet.GetRow(row).GetCell(25).StringCellValue;
                        if (sd == null)
                            shippingAdviceVM.DeliveryDate = DateTime.Now;
                        else
                        shippingAdviceVM.DeliveryDate = DateTime.Parse(sd, new CultureInfo("en-IN", true));
                        currentMothTimeSheetList.Add(shippingAdviceVM);

                        //  }
                        //Here for sample , I just save the value in "value" field, Here you can write your custom logics...  
                    }
                }
                if (currentMothTimeSheetList.Count() > 0)
                {
                    trade.SaveShippingAdvice(currentMothTimeSheetList);
                    TempData["SuccessMessage"] = "ShippingAdvice data appended to DB";
                }
            }
            catch (Exception ex)
            {
                //TempData["ErrorMessage"] = "There is a Issues on Uploading Workers OT Data .Please contact IT Support";
                //Console.Write(ex.Message);
                throw ex;
            }
        }

        public string getValueFromCell(ICell cell, XSSFFormulaEvaluator formula)
        {
            string output = "";
            if (cell != null)
            {

                switch (cell.CellType)
                {
                    case CellType.String:
                        output = cell.StringCellValue;
                        break;
                    case CellType.Numeric:

                        output = cell.NumericCellValue.ToString();
                        break;
                    case CellType.Boolean:
                        output = cell.BooleanCellValue.ToString();

                        break;

                    case CellType.Formula:
                        //     formula.EvaluateAll();
                        output = cell.NumericCellValue.ToString();

                        break;

                    default:
                        output = "";
                        break;
                }
            }

            return output;
        }

        private void ProcessInputFile(string fileName)
        {
            using (Trade tradeSheet = new Trade())
            {
                tradeSheet.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
                var tradeCostsSheet = tradeSheet.LoadSheet(fileName, 0);
                if (tradeSheet.ValidateSheet(tradeCostsSheet))
                {
                    tradeSheet.ProcessSheet(tradeCostsSheet);
                }
            }
        }
    }
}