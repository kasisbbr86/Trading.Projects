using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Collections.Generic;
using System.Dynamic;
using Trading.BLL.Models;
using System.Reflection;
using System.Xml.Linq;
using System.Linq;

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

        private readonly XDocument ShippingModelDocument;
        public Trade()
        {
            ShippingModelDocument = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Trading.BLL.ImportConfiguration.xml"));
        }

        public ExpandoObject LoadSheet(string sheetFullPath, int sheetIndex) 
        {
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

            uploadTradeDetails.Shipping = GetShipping(sheetDetails.Sheet, sheetDetails.ShippingDetailsRowIndex, sheetDetails.DocumentInstructionsRowIndex);
            uploadTradeDetails.DocumentInstructions = GetDocumentInstructions(sheetDetails.Sheet, sheetDetails.DocumentInstructionsRowIndex);
            uploadTradeDetails.ShippingModels = GetShippingModels(sheetDetails.Sheet, sheetDetails.ShippingModelsRowIndex);
            
            // save data
        }

        private Shipping GetShipping(ISheet tradeSheet, int shippingDetailsRowIndex, int nextSectionRowIndex)
        {
            Shipping shippingDetail = new Shipping();
            IRow tradeSheetRow;
            for (int shippingRowCounter = shippingDetailsRowIndex + 1; shippingRowCounter < nextSectionRowIndex; shippingRowCounter++)
            {
                tradeSheetRow = tradeSheet.GetRow(shippingRowCounter);
                if (tradeSheetRow == null) continue;
                string shippingElementKey = tradeSheetRow.Cells[0].ToString().Replace(":", "").Trim() // check the characters in next line
                                                .Replace("/", "-").Replace(" ", "-").Replace("*", "").Replace(",", "").Trim(); // first cell of each row
                if (shippingElementKey != string.Empty)
                {
                    bool hasShippingElement = ShippingModelDocument.Root.Descendants("Shipping").Elements(shippingElementKey).Any();
                    if (hasShippingElement)
                    {
                        PropertyInfo propertyInfo = shippingDetail.GetType().GetProperty(shippingElementKey.Replace("-", ""));
                        string propertyValue = tradeSheetRow.Cells[1].ToString(); // to do check
                        propertyInfo.SetValue(shippingDetail, propertyValue, null);
                    }
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
                foreach (ICell item in tradeSheetRow.Cells.FindAll(cell => cell.CellType == CellType.String && cell.StringCellValue != string.Empty))
                {
                    documentInstructionList.Add( new DocumentInstruction() { Instruction = item.StringCellValue } );
                }
            }
            return documentInstructionList;
        }

        private List<ShippingModel> GetShippingModels(ISheet tradeSheet, int shippingModelsRowIndex)
        {
            List<ShippingModel> shippingModelList = new List<ShippingModel>();            
            IRow tradeSheetRow;
            int headerRowIndex = -1;
            for (int rowCounter = shippingModelsRowIndex + 1; rowCounter <= tradeSheet.LastRowNum; rowCounter++)
            {
                tradeSheetRow = tradeSheet.GetRow(rowCounter);
                if (tradeSheetRow == null) continue;
                if (headerRowIndex > 0
                    && tradeSheetRow.Cells.FindAll(d => d.CellType == CellType.String && d.StringCellValue.Contains("TOTAL")).Count == 0)
                {
                    ShippingModel shippingModel = new ShippingModel();
                    // ToDo: validate, configure indexes

                    IEnumerable<XNode> modelColumns = ShippingModelDocument.Root.Descendants("ShippingModels").Descendants("Columns").DescendantNodes();
                    foreach (XElement column in modelColumns)
                    {
                        PropertyInfo propertyInfo = shippingModel.GetType().GetProperty(column.Name.ToString());
                        string propertyValue = tradeSheetRow.Cells[Convert.ToInt32(column.Attribute("Index").Value)].ToString();
                        propertyInfo.SetValue(shippingModel, propertyValue, null);
                    }
                    shippingModelList.Add(shippingModel);
                }
                else if (tradeSheetRow.Cells.FindAll(d => d.CellType == CellType.String && d.StringCellValue.Contains("P/O NO")).Count > 0)
                {
                    headerRowIndex = rowCounter;
                    //ShippingModelDocument.Root.Descendants("ShippingModels").
                }
                else if (tradeSheetRow.Cells.FindAll(d => d.CellType == CellType.String && d.StringCellValue.Contains("TOTAL")).Count > 0)
                {
                    break;
                }
            }
            return shippingModelList;

        }
    }
}
