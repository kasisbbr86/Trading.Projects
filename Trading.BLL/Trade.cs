using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Collections.Generic;
using System.Dynamic;
using Trading.DAO;
using System.Reflection;
using System.Xml.Linq;
using System.Linq;
using Newtonsoft.Json;
using System.Data;
using Trading.Utilities;

namespace Trading.BLL
{
    public class Trade: IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private const string ShippingDetailsRowPlaceholder = "///  SHIPPING  INSTRUCTION  ///";
        private const string DocumentInstructionsRowPlaceholder = "*****INSTRUCTION FOR DOCUMENTS*****";
        private const string ShippingModelsRowPlaceholder = "*****INSTRUCTIONS FOR SHIPPING MODELS*****";

        private readonly XDocument _shippingModelDocument;
        private readonly TradeLogger _tradeLogger = new TradeLogger();
        public string TradeDBConnectionString { get; set; }

        public Trade()
        {
            _shippingModelDocument = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Trading.BLL.ImportConfiguration.xml"));
        }

        public ExpandoObject LoadSheet(string sheetFullPath, int sheetIndex) 
        {
            // http://apache-poi.1045710.n5.nabble.com/How-to-check-for-valid-excel-files-using-POI-without-checking-the-file-extension-td2341055.html
            ISheet tradeSheet;
            try
            {
                using (FileStream FS = new FileStream(sheetFullPath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook tradingWorkbook = WorkbookFactory.Create(FS);
                    tradeSheet = tradingWorkbook.GetSheetAt(sheetIndex);
                }
            }
            catch (Exception ex)
            {
                _tradeLogger.Error("Trading.BLL.Trade.LoadSheet", ex);
                throw ex;
            }

            dynamic sheetDetails = new ExpandoObject();
            sheetDetails.Sheet = tradeSheet;
            sheetDetails.FirstRowIndex = tradeSheet.FirstRowNum;
            sheetDetails.LastRowIndex = tradeSheet.LastRowNum;
            sheetDetails.ShippingDetailsRowIndex = -1;
            sheetDetails.DocumentInstructionsRowIndex = -1;
            sheetDetails.ShippingModelsRowIndex = -1;

            try
            {
                IRow tradeSheetRow;
                for (int rowCounter = 0; rowCounter < tradeSheet.LastRowNum; rowCounter++)
                {
                    tradeSheetRow = tradeSheet.GetRow(rowCounter);
                    if (tradeSheetRow == null) continue;
                    if (tradeSheetRow.Cells.FindAll(d => d.CellType == CellType.String && d.StringCellValue.Contains(ShippingDetailsRowPlaceholder)).Count > 0)
                    {
                        sheetDetails.ShippingDetailsRowIndex = rowCounter;
                    }
                    else if (tradeSheetRow.Cells.FindAll(d => d.CellType == CellType.String && d.StringCellValue.Contains(DocumentInstructionsRowPlaceholder)).Count > 0)
                    {
                        sheetDetails.DocumentInstructionsRowIndex = rowCounter;
                    }
                    else if (tradeSheetRow.Cells.FindAll(d => d.CellType == CellType.String && d.StringCellValue.Contains(ShippingModelsRowPlaceholder)).Count > 0)
                    {
                        sheetDetails.ShippingModelsRowIndex = rowCounter;
                    }
                }
            }
            catch (Exception ex)
            {
                _tradeLogger.Error("Trading.BLL.Trade.LoadSheet", ex);
                throw ex;
            }
            return sheetDetails;
        }

        public bool ValidateSheet(dynamic sheetDetails)
        {
            bool isValid = true;

            // Check if all the sections are available in the Input Sheet
            if (sheetDetails.Sheet == null || sheetDetails.FirstRowIndex == -1 || sheetDetails.LastRowIndex == -1
                || sheetDetails.ShippingDetailsRowIndex == -1 || sheetDetails.DocumentInstructionsRowIndex == -1 || sheetDetails.ShippingModelsRowIndex == -1)
            {
                isValid = false;
            }
            return isValid;
        }

        public void ProcessSheet(dynamic sheetDetails) 
        {
            UploadTrade uploadTradeDetails = new UploadTrade();
            UploadTradeLog uploadTradeLog = new UploadTradeLog() { ImportDate = DateTime.Now, WorkBookName = sheetDetails.Sheet.SheetName, TradeRequest = string.Empty, ExceptionMessage = string.Empty };
            DAL.Trade trade = new DAL.Trade();
            trade.TradeDBConnectionString = this.TradeDBConnectionString;
            int shippingId = -1;
            string shippingMissedElements = string.Empty;
            try
            {
                uploadTradeDetails.Shipping = GetShipping(sheetDetails.Sheet, sheetDetails.ShippingDetailsRowIndex, sheetDetails.DocumentInstructionsRowIndex);
                uploadTradeDetails.Shipping.TradeSheetName = sheetDetails.Sheet.SheetName;
                uploadTradeDetails.DocumentInstructions = GetDocumentInstructions(sheetDetails.Sheet, sheetDetails.DocumentInstructionsRowIndex);
                uploadTradeDetails.ShippingModels = GetShippingModels(sheetDetails.Sheet, sheetDetails.ShippingModelsRowIndex);

                if (uploadTradeDetails.Shipping.SINo.Trim() != string.Empty)
                {
                    shippingMissedElements = "The SI No is not avaialable. Please include and process again.";
                }
                else
                {
                    DataTable documentInstructionsTable = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(uploadTradeDetails.DocumentInstructions));
                    DataTable shippingModelsTable = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(uploadTradeDetails.ShippingModels));
                    shippingId = trade.SaveShippingTradeDetails(uploadTradeDetails.Shipping, documentInstructionsTable, shippingModelsTable);
                }
                if (shippingId > 0)
                {
                    uploadTradeLog.ShippingId = shippingId;
                    uploadTradeLog.ImportStatus = "IMPORTED";
                    trade.WriteShippingImportLog(uploadTradeLog);
                }
                else
                {
                    uploadTradeLog.ShippingId = shippingId;
                    uploadTradeLog.ImportStatus = "VALIDATIONS";
                    uploadTradeLog.ExceptionMessage = shippingMissedElements;
                    trade.WriteShippingImportLog(uploadTradeLog);
                }
            }
            catch (Exception ex)
            {
                uploadTradeLog.ShippingId = shippingId;
                uploadTradeLog.ImportStatus = "FAILED";
                uploadTradeLog.ExceptionMessage = ex.Message;
                trade.WriteShippingImportLog(uploadTradeLog);
                _tradeLogger.Error("Trading.BLL.Trade.LoadSheet", ex);
                throw ex;
            }
        }

        public UploadTrade GetShippingTradeDetails(int shippingId)
        {
            DAL.Trade trade = new DAL.Trade();
            trade.TradeDBConnectionString = this.TradeDBConnectionString;
            return trade.GetShippingTradeDetails(shippingId);
        }

        private Shipping GetShipping(ISheet tradeSheet, int shippingDetailsRowIndex, int nextSectionRowIndex)
        {
            Shipping shippingDetail = new Shipping();
            IRow tradeSheetRow;
            for (int shippingRowCounter = shippingDetailsRowIndex + 1; shippingRowCounter < nextSectionRowIndex; shippingRowCounter++)
            {
                tradeSheetRow = tradeSheet.GetRow(shippingRowCounter);
                if (tradeSheetRow == null) continue;
                try
                {
                    string shippingElementKey = tradeSheetRow.Cells[0].ToString().Replace(":", "").Trim(); // first cell of each row
                    if (shippingElementKey != string.Empty)
                    {
                        IEnumerable<XElement> shippingProperty = _shippingModelDocument.Root.Descendants("Shipping").Descendants("Properties")
                                                    .Elements("Property").Where(x => shippingElementKey.Contains(x.Attribute("RowLabel").Value));
                        if (shippingProperty.Any())
                        {
                            PropertyInfo propertyInfo = shippingDetail.GetType().GetProperty(shippingProperty.First().Attribute("DBColumn").Value);
                            propertyInfo.SetValue(shippingDetail, string.Empty, null);
                            if (tradeSheetRow.Cells.ElementAtOrDefault(1) != null)
                            {
                                string propertyValue = tradeSheetRow.Cells[1].ToString(); // to do check
                                propertyInfo.SetValue(shippingDetail, propertyValue, null);
                            }                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    _tradeLogger.Error("Trading.BLL.Trade.LoadSheet", ex);
                    throw ex;
                }   
            }
            return shippingDetail;
        }

        private List<DocumentInstruction> GetDocumentInstructions(ISheet tradeSheet, int documentInstructionsRowIndex)
        {
            List<DocumentInstruction> documentInstructionList = new List<DocumentInstruction>();
            IRow tradeSheetRow;
            for (int instructionIndex = documentInstructionsRowIndex + 1; instructionIndex <= documentInstructionsRowIndex + 3; instructionIndex++)
            {
                tradeSheetRow = tradeSheet.GetRow(instructionIndex);
                if (tradeSheetRow == null) continue;
                try
                {
                    foreach (ICell item in tradeSheetRow.Cells.FindAll(cell => cell.CellType == CellType.String && cell.StringCellValue != string.Empty))
                    {
                        documentInstructionList.Add(new DocumentInstruction() { Instruction = item.StringCellValue });
                    }
                }
                catch (Exception ex)
                {
                    _tradeLogger.Error("Trading.BLL.Trade.LoadSheet", ex);
                    throw ex;
                }
            }
            return documentInstructionList;
        }

        private List<ShippingModel> GetShippingModels(ISheet tradeSheet, int shippingModelsRowIndex)
        {
            List<ShippingModel> shippingModelList = new List<ShippingModel>();            
            IRow tradeSheetRow;
            List<XElement> columnsMetaData = new List<XElement>();
            int headerRowIndex = -1;
            for (int rowCounter = shippingModelsRowIndex + 1; rowCounter <= tradeSheet.LastRowNum; rowCounter++)
            {
                tradeSheetRow = tradeSheet.GetRow(rowCounter);
                if (tradeSheetRow == null) continue;
                try
                {
                    if (headerRowIndex > 0
                                && tradeSheetRow.Cells.FindAll(d => d.CellType == CellType.String && d.StringCellValue.Contains("TOTAL")).Count == 0)
                    {
                        ShippingModel shippingModel = new ShippingModel();
                        List<XElement> modelColumns = columnsMetaData; // workaround. ToDo: Need to have a column benchmark
                        try
                        {
                            // [START] workaround. ToDo: Need to have a column benchmark
                            if (tradeSheetRow.Cells.Count != columnsMetaData.Count && tradeSheetRow.Cells.Count == 6) // version not present in the row.
                            {
                                for (int decrementCounterIndex = 3; decrementCounterIndex < columnsMetaData.Count; decrementCounterIndex++)
                                {
                                    modelColumns[decrementCounterIndex].Attribute("Index").Value = (decrementCounterIndex - 1).ToString();
                                }
                            }
                            // [END] workaround. ToDo: Need to have a column benchmark
                            foreach (var modelColumn in modelColumns)
                            {
                                if (modelColumn.Attribute("Index").Value != string.Empty)
                                {
                                    PropertyInfo propertyInfo = shippingModel.GetType().GetProperty(modelColumn.Attribute("DBColumn").Value.ToString());
                                    string propertyValue = tradeSheetRow.Cells[Convert.ToInt32(modelColumn.Attribute("Index").Value)].ToString();
                                    propertyInfo.SetValue(shippingModel, propertyValue, null);
                                }
                            }
                            shippingModelList.Add(shippingModel);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else if (tradeSheetRow.Cells.FindAll(d => d.CellType == CellType.String && d.StringCellValue.Contains("P/O NO")).Count > 0)
                    {
                        headerRowIndex = rowCounter;
                        columnsMetaData = _shippingModelDocument.Root.Descendants("ShippingModels").Descendants("Column").ToList();
                        for (int columnIndex = 0; columnIndex < tradeSheetRow.Cells.Count; columnIndex++)
                        {
                            XElement headerColumnElement = columnsMetaData
                                                                .Where(column => tradeSheetRow.Cells[columnIndex].ToString().Trim().Equals(column.Attribute("ColumnHeader").Value))
                                                                .FirstOrDefault();
                            if (headerColumnElement != null) headerColumnElement.Attribute("Index").Value = columnIndex.ToString();
                        }
                    }
                    else if (tradeSheetRow.Cells.FindAll(d => d.CellType == CellType.String && d.StringCellValue.Contains("TOTAL")).Count > 0)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _tradeLogger.Error("Trading.BLL.Trade.LoadSheet", ex);
                    throw ex;
                }
            }
            return shippingModelList;

        }
    }
}
