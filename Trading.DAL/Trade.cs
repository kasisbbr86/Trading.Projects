using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Trading.DAO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trading.Utilities;
using Trading.ViewModel;
using System.IO;
using System.Xml.Serialization;

namespace Trading.DAL
{
    public class Trade
    {
        public string TradeDBConnectionString { get; set; }
        private readonly TradeLogger _tradeLogger = new TradeLogger();

        public Trade()
        {
        }

        public int SaveShippingTradeDetails(DAO.Shipping shipping, DataTable documentInstructionsTable, DataTable shippingModelsTable)
        {
            SqlParameter parameter;
            int shippingId = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SaveShippingTradeDetails", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (var property in shipping.GetType().GetProperties())
                    {
                        parameter = new SqlParameter();
                        parameter.ParameterName = "@" + property.Name;
                        parameter.SqlDbType = SqlDbType.NVarChar;
                        parameter.Value = property.GetValue(shipping, null);
                        cmd.Parameters.Add(parameter);
                    }

                    parameter = new SqlParameter();
                    parameter.ParameterName = "@tvpDocumentInstructions";
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.Value = documentInstructionsTable;
                    cmd.Parameters.Add(parameter);

                    parameter = new SqlParameter();
                    parameter.ParameterName = "@tvpShippingModels";
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.Value = shippingModelsTable;
                    cmd.Parameters.Add(parameter);

                    shippingId = (int)cmd.ExecuteScalar();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _tradeLogger.Error("Trading.DAL.Trade.LoadSheet", ex);
                throw ex;
            }
            return shippingId;
        }
        public int SaveCourierMasterAndDetails(IncomingCourierMasterVM incomingCourierMasterVM, List<IncomingCourierDetailsVM> incomingCourierDetailsVM)
        {
            int MasterID = SaveIncomingCourierMaster(incomingCourierMasterVM);
            incomingCourierDetailsVM.ForEach(x => x.MasterID = MasterID);
            DoXmlChanges(incomingCourierDetailsVM);
            return MasterID;
        }

        public int SaveIncomingCourierMaster(IncomingCourierMasterVMSave incomingCourierMasterVM)
        {
            SqlParameter parameter;
            int courierMasterId = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SaveIncomingCourierMaster", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (var property in incomingCourierMasterVM.GetType().GetProperties())
                    {
                        parameter = new SqlParameter();
                        parameter.ParameterName = "@" + property.Name;
                        switch (Type.GetTypeCode(property.PropertyType))
                        {
                            case TypeCode.Int32:
                                parameter.SqlDbType = SqlDbType.Int;
                                break;
                            case TypeCode.String:
                                parameter.SqlDbType = SqlDbType.NVarChar;
                                break;
                            case TypeCode.DateTime:
                                parameter.SqlDbType = SqlDbType.DateTime;
                                break;
                            default:
                                parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                                break;
                        }
                        parameter.Value = property.GetValue(incomingCourierMasterVM, null);
                        cmd.Parameters.Add(parameter);
                    }

                    courierMasterId = (int)cmd.ExecuteScalar();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _tradeLogger.Error("Trading.DAL.Trade.LoadSheet", ex);
                throw ex;
            }
            return courierMasterId;
        }

        public int SaveIncomingCourierDetails(DataTable incomingCourierDetailsVMTable)
        {
            SqlParameter parameter;
            int shippingId = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SaveIncomingCourierDetails", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    parameter = new SqlParameter();
                    parameter.ParameterName = "@tvpIncomingCourierDetails";
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.Value = incomingCourierDetailsVMTable;
                    cmd.Parameters.Add(parameter);

                    shippingId = (int)cmd.ExecuteScalar();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _tradeLogger.Error("Trading.DAL.Trade.LoadSheet", ex);
                throw ex;
            }
            return shippingId;
        }

        public void DoXmlChanges(List<IncomingCourierDetailsVM> salesCommonDetails)
        {
            int shippingId = -1;
            string errorMsg = "";
            try
            {
                if (salesCommonDetails.Count > 0)
                {
                    string xml = null;
                    using (StringWriter sw = new StringWriter())
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(List<IncomingCourierDetailsVM>));
                        xs.Serialize(sw, salesCommonDetails);
                        xml = sw.ToString();
                    }
                    using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("SaveIncomingCourier", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter parameter;

                        parameter = new SqlParameter();
                        parameter.ParameterName = "@xmldata";
                        parameter.SqlDbType = SqlDbType.Xml;
                        parameter.Value = xml;
                        cmd.Parameters.Add(parameter);

                        parameter = new SqlParameter();
                        parameter.ParameterName = "@status";
                        parameter.SqlDbType = SqlDbType.NVarChar;
                        parameter.Value = "";
                        cmd.Parameters.Add(parameter);
                        cmd.Parameters["@status"].Direction = ParameterDirection.Output;
                        shippingId = (int)cmd.ExecuteScalar();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.InnerException.Message.Split('\r')[0];
                throw new Exception(errorMsg);
            }
        }

        public bool CheckIsSINoAlreadyExist(string siNo)
        {
            bool isAlreadyExist = false;
            using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Shipping where SINo=" + siNo;

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataSet tradeAndShippingDetails = new DataSet();
                adapter.Fill(tradeAndShippingDetails);

                if (tradeAndShippingDetails.Tables[0].Rows.Count > 0)
                {
                    isAlreadyExist = true;

                }
                connection.Close();
            }
            return isAlreadyExist;
        }

        public void SaveShippingAdvice(List<ShippingAdviceVM> shippingAdvice)
        {
            try
            {
                for (int ind = 0; ind <= shippingAdvice.Count-1; ind++)
                {
                    string values= string.Format("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}'", shippingAdvice[ind].CompanyID, shippingAdvice[ind].SCInvoiceNo, shippingAdvice[ind].InvoiceAmount, shippingAdvice[ind].Consignee, shippingAdvice[ind].BLDate, shippingAdvice[ind].ReceivedDate, shippingAdvice[ind].Shiper, shippingAdvice[ind].BLNo, shippingAdvice[ind].Factory, shippingAdvice[ind].Department, shippingAdvice[ind].Material, shippingAdvice[ind].Quantity, shippingAdvice[ind].FOB, shippingAdvice[ind].PurchaseDocumentNo, shippingAdvice[ind].Item1, shippingAdvice[ind].SAPSO, shippingAdvice[ind].Item2, shippingAdvice[ind].SAPDO, shippingAdvice[ind].PInt, shippingAdvice[ind].SLoc, shippingAdvice[ind].Temp1, shippingAdvice[ind].Seq, shippingAdvice[ind].Del, shippingAdvice[ind].Comp, shippingAdvice[ind].DeliveryDate);
                using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                    {
                        connection.Open();
                        SqlCommand sCommand = new SqlCommand();
                        sCommand.CommandText = "Insert into ShippingAdvice(CompanyID,SCInvoiceNo,InvoiceAmount,Consignee,BLDate,ReceivedDate,Shiper,BLNo,Factory,Department,Material,Quantity,FOB,PurchaseDocumentNo,Item1,SAPSO,Item2,SAPDO,PInt,SLoc,Temp1,Seq,Del,Comp,DeliveryDate)values" + "("+ values+")";
                        sCommand.CommandType = CommandType.Text;
                        sCommand.Connection = connection;
                        sCommand.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public int DeleteCourierInvoice(int invoiceDetailID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                {
                    connection.Open();
                    SqlCommand sCommand = new SqlCommand();
                    sCommand.CommandText = "DeleteCourierInvoice";
                    sCommand.CommandType = CommandType.StoredProcedure;
                    sCommand.Connection = connection;

                    SqlParameter parameter = new SqlParameter();
                    parameter.ParameterName = "@InvoiceDetailId";
                    parameter.SqlDbType = SqlDbType.Int;
                    parameter.Value = invoiceDetailID;
                    sCommand.Parameters.Add(parameter);

                    int recordDeleted = sCommand.ExecuteNonQuery();
                    connection.Close();
                    return recordDeleted;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void WriteShippingImportLog(UploadTradeLog uploadTradeLog)
        {
            SqlParameter parameter;
            try
            {
                using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("WriteShippingImportLog", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    parameter = new SqlParameter();
                    parameter.ParameterName = "@ShippingId";
                    parameter.SqlDbType = SqlDbType.Int;
                    parameter.Value = uploadTradeLog.ShippingId;
                    cmd.Parameters.Add(parameter);

                    parameter = new SqlParameter();
                    parameter.ParameterName = "@WorkBookName";
                    parameter.SqlDbType = SqlDbType.NVarChar;
                    parameter.Value = uploadTradeLog.WorkBookName;
                    cmd.Parameters.Add(parameter);

                    parameter = new SqlParameter();
                    parameter.ParameterName = "@TradeRequest";
                    parameter.SqlDbType = SqlDbType.NVarChar;
                    parameter.Value = uploadTradeLog.TradeRequest;
                    cmd.Parameters.Add(parameter);

                    parameter = new SqlParameter();
                    parameter.ParameterName = "@ImportDate";
                    parameter.SqlDbType = SqlDbType.DateTime;
                    parameter.Value = uploadTradeLog.ImportDate;
                    cmd.Parameters.Add(parameter);

                    parameter = new SqlParameter();
                    parameter.ParameterName = "@ImportStatus";
                    parameter.SqlDbType = SqlDbType.NVarChar;
                    parameter.Value = uploadTradeLog.ImportStatus;
                    cmd.Parameters.Add(parameter);

                    parameter = new SqlParameter();
                    parameter.ParameterName = "@ExceptionMessage";
                    parameter.SqlDbType = SqlDbType.NVarChar;
                    parameter.Value = uploadTradeLog.ExceptionMessage;
                    cmd.Parameters.Add(parameter);

                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _tradeLogger.Error("Trading.DAL.Trade.LoadSheet", ex);
                throw ex;
            }
        }

        public UploadTradeVM GetShippingTradeDetails(int shippingId)
        {
            UploadTradeVM uploadedTrade = new UploadTradeVM();
            SqlParameter parameter;
            try
            {
                using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("GetShippingTradeDetails", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    parameter = new SqlParameter();
                    parameter.ParameterName = "@SINo";
                    parameter.SqlDbType = SqlDbType.Int;
                    parameter.Value = shippingId;
                    cmd.Parameters.Add(parameter);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet tradeAndShippingDetails = new DataSet();
                    adapter.Fill(tradeAndShippingDetails, "Trade");

                    uploadedTrade = new UploadTradeVM()
                    {
                        Shipping = new ViewModel.Shipping(),
                        DocumentInstructions = new List<DocumentInstructionVM>(),
                        ShippingModels = new List<ShippingModelVM>()
                    };
                    uploadedTrade.ShippingId = shippingId;
                    if (tradeAndShippingDetails.Tables[0].Rows.Count > 0)
                    {
                        uploadedTrade.Shipping = (from DataRow dr in tradeAndShippingDetails.Tables[0].Rows
                                                  select new ViewModel.Shipping
                                                  {
                                                      TradeSheetName = dr["TradeSheetName"]?.ToString(),
                                                      BLConsignee = dr["BLConsignee"]?.ToString(),
                                                      FinalDestination = dr["FinalDestination"]?.ToString(),
                                                      Freight = dr["Freight"]?.ToString(),
                                                      LCExpiryDate = dr["LCExpiryDate"]?.ToString(),
                                                      LCIssuanceDate = dr["LCIssuanceDate"]?.ToString(),
                                                      LCIssuingBank = dr["LCIssuingBank"]?.ToString(),
                                                      LCNo = dr["LCNo"]?.ToString(),
                                                      PartialShipment = dr["PartialShipment"]?.ToString(),
                                                      PaymentTerms = dr["PaymentTerms"]?.ToString(),
                                                      PortOfDischarge = dr["PortOfDischarge"]?.ToString(),
                                                      PortOfLoading = dr["PortOfLoading"]?.ToString(),
                                                      RequiredBLDate = dr["RequiredBLDate"]?.ToString(),
                                                      ShipmentExpiryDate = dr["ShipmentExpiryDate"]?.ToString(),
                                                      ShipToParty = dr["ShipToParty"]?.ToString(),
                                                      SIDate = dr["SIDate"]?.ToString(),
                                                      SINo = dr["SINo"]?.ToString(),
                                                      SoldToParty = dr["SoldToParty"]?.ToString(),
                                                      TradeTerms = dr["TradeTerms"]?.ToString(),
                                                      Transportation = dr["Transportation"]?.ToString(),
                                                      TransShipment = dr["TransShipment"]?.ToString(),
                                                      Vender = dr["Vender"]?.ToString(),
                                                      Via = dr["Via"]?.ToString()
                                                  }).FirstOrDefault();
                    }

                    if (tradeAndShippingDetails.Tables[1].Rows.Count > 0)
                    {
                        uploadedTrade.DocumentInstructions = (from DataRow dr in tradeAndShippingDetails.Tables[1].Rows
                                                              select new DocumentInstructionVM
                                                              {
                                                                  Instruction = dr["Instruction"]?.ToString()
                                                              }).ToList();
                    }

                    if (tradeAndShippingDetails.Tables[2].Rows.Count > 0)
                    {
                        uploadedTrade.ShippingModels = (from DataRow dr in tradeAndShippingDetails.Tables[2].Rows
                                                        select new ShippingModelVM
                                                        {
                                                            PONo = dr["PONo"]?.ToString(),
                                                            BLModelName = dr["BLModelName"]?.ToString(),
                                                            Description = dr["Description"]?.ToString(),
                                                            ModelName = dr["ModelName"]?.ToString(),
                                                            Quantity = dr["Quantity"]?.ToString(),
                                                            Remarks = dr["Remarks"]?.ToString(),
                                                            Version = dr["Version"]?.ToString()
                                                        }).ToList();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _tradeLogger.Error("Trading.DAL.Trade.LoadSheet", ex);
                throw ex;
            }
            return uploadedTrade;
        }


        public IncomingCourierMasterVM GetCourierMasterDetails(int masterID)
        {
            IncomingCourierMasterVM objIncomingCourierMaster = new IncomingCourierMasterVM();
            List<IncomingCourierDetailsVM> lstIncomingCourierDetails = new List<IncomingCourierDetailsVM>();

            try
            {
                using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM IncomingCourierMaster where MasterID=" + masterID;

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataSet incomingCourierMaster = new DataSet();
                    adapter.Fill(incomingCourierMaster);
                    if (incomingCourierMaster.Tables[0].Rows.Count > 0)
                    {
                        objIncomingCourierMaster = (from DataRow dr in incomingCourierMaster.Tables[0].Rows
                                                    select new ViewModel.IncomingCourierMasterVM
                                                    {
                                                        CompanyID = Guid.Parse(dr["CompanyID"].ToString()),
                                                        MasterID = (int)dr["MasterID"],
                                                        AWBNo = dr["AWBNo"]?.ToString(),
                                                        CourierCompany = (int)dr["CourierCompany"],
                                                        ReceivedFrom = (int)dr["ReceivedFrom"],
                                                        CourierFor = (int)dr["CourierFor"],
                                                        DocumentType = (int)dr["DocumentType"],
                                                        ReceivedOn = (DateTime)dr["ReceivedOn"],
                                                        HandedOverOn = (DateTime)dr["HandedOverOn"]
                                                    }).FirstOrDefault();
                    }

                    string documentDetailsQuery = "SELECT * FROM IncomingCourierDetails WHERE MasterID=" + masterID;
                    SqlDataAdapter documentDetailsAdapter = new SqlDataAdapter(documentDetailsQuery, connection);
                    DataSet incomingCourierDetails = new DataSet();
                    documentDetailsAdapter.Fill(incomingCourierDetails);

                    if (incomingCourierDetails.Tables[0].Rows.Count > 0)
                    {
                        lstIncomingCourierDetails = (from DataRow dr in incomingCourierDetails.Tables[0].Rows
                                                     select new ViewModel.IncomingCourierDetailsVM
                                                     {
                                                         CompanyID = Guid.Parse(dr["CompanyID"].ToString()),
                                                         MasterID = (int)dr["MasterID"],
                                                         DetailsID = (int)dr["DetailsID"],
                                                         DocDetail = dr["DocDetail"]?.ToString(),
                                                         RefNo = dr["RefNo"]?.ToString(),
                                                         Qty = (decimal)dr["Qty"],
                                                         Remarks = dr["Remarks"]?.ToString(),
                                                         FileName = dr["FileName"]?.ToString(),
                                                         FilePath = dr["FilePath"]?.ToString(),
                                                         ParentDetailsID = (int)dr["ParentDetailsID"],
                                                         IsSubDetail = (bool)dr["IsSubDetail"],
                                                         SINo = dr["SINo"]?.ToString(),
                                                     }).ToList();
                    }
                    if (objIncomingCourierMaster != null)
                    {
                        objIncomingCourierMaster.CourierDetails = lstIncomingCourierDetails;
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _tradeLogger.Error("Trading.DAL.Trade.LoadSheet", ex);
                throw ex;
            }
            return objIncomingCourierMaster;
        }

        public IncomingCourierMasterVM GetCourierDocumentDetails(int parentDetailID)
        {
            IncomingCourierMasterVM objIncomingCourierMaster = new IncomingCourierMasterVM();
            List<IncomingCourierDetailsVM> lstIncomingCourierDetails = new List<IncomingCourierDetailsVM>();
            try
            {
                using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                {
                    connection.Open();
                    string documentDetailsQuery = "SELECT * FROM IncomingCourierDetails WHERE ParentDetailsID=" + parentDetailID;
                    
                    SqlDataAdapter documentDetailsAdapter = new SqlDataAdapter(documentDetailsQuery, connection);
                    DataSet incomingCourierDetails = new DataSet();
                    documentDetailsAdapter.Fill(incomingCourierDetails);

                    if (incomingCourierDetails.Tables[0].Rows.Count > 0)
                    {
                        lstIncomingCourierDetails = (from DataRow dr in incomingCourierDetails.Tables[0].Rows
                                                     select new ViewModel.IncomingCourierDetailsVM
                                                     {
                                                         CompanyID = Guid.Parse(dr["CompanyID"].ToString()),
                                                         MasterID = (int)dr["MasterID"],
                                                         DetailsID = (int)dr["DetailsID"],
                                                         DocDetail = dr["DocDetail"]?.ToString(),
                                                         RefNo = dr["RefNo"]?.ToString(),
                                                         Qty = (decimal)dr["Qty"],
                                                         Remarks = dr["Remarks"]?.ToString(),
                                                         FileName = dr["FileName"]?.ToString(),
                                                         FilePath = dr["FilePath"]?.ToString(),
                                                         ParentDetailsID = (int)dr["ParentDetailsID"],
                                                         IsSubDetail = (bool)dr["IsSubDetail"],
                                                         SINo = dr["SINo"]?.ToString(),
                                                     }).ToList();
                    }
                    if (objIncomingCourierMaster != null)
                    {
                        objIncomingCourierMaster.CourierDetails = lstIncomingCourierDetails;
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _tradeLogger.Error("Trading.DAL.Trade.LoadSheet", ex);
                throw ex;
            }
            return objIncomingCourierMaster;
        }

        public List<Courier> GetCourierList()
        {
            List<Courier> objIncomingCourierMaster = new List<Courier>();
            try
            {
                using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM IncomingCourierMaster";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataSet incomingCouriers = new DataSet();
                    adapter.Fill(incomingCouriers);

                    if (incomingCouriers.Tables[0].Rows.Count > 0)
                    {
                        objIncomingCourierMaster = (from DataRow dr in incomingCouriers.Tables[0].Rows
                                                    select new Courier
                                                    {
                                                        Id = (int)dr["MasterID"],
                                                        AWBNumber = dr["AWBNo"]?.ToString(),
                                                        ReceivedFrom = (DateTime)dr["HandedOverOn"],
                                                        ReceivedOn = (DateTime)dr["ReceivedOn"]
                                                        
                                                    }).ToList();
                    }
                    
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _tradeLogger.Error("Trading.DAL.Trade.LoadSheet", ex);
                throw ex;
            }
            return objIncomingCourierMaster;
        }

        public List<ViewModel.ShippingAdviceVM> GetShippingAdvice()
        {
            List<ViewModel.ShippingAdviceVM> lstShippingAdvice = new List<ViewModel.ShippingAdviceVM>();
            // SqlParameter parameter;
            try
            {
                using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM ShippingAdvice";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataSet tradeAndShippingAdviceDetails = new DataSet();
                    adapter.Fill(tradeAndShippingAdviceDetails);



                    if (tradeAndShippingAdviceDetails.Tables[0].Rows.Count > 0)
                    {
                        lstShippingAdvice = (from DataRow dr in tradeAndShippingAdviceDetails.Tables[0].Rows
                                       select new ViewModel.ShippingAdviceVM
                                       {
                                           SCInvoiceNo = dr["SCInvoiceNo"]?.ToString(),
                                           InvoiceAmount = (Decimal)dr["InvoiceAmount"],
                                           Consignee = dr["Consignee"]?.ToString(),
                                           BLDate = (DateTime)dr["BLDate"],
                                           ReceivedDate = (DateTime)dr["ReceivedDate"],
                                           Shiper = dr["Shiper"]?.ToString(),
                                           BLNo = dr["BLNo"]?.ToString(),
                                           Factory = dr["Factory"]?.ToString(),
                                           Department = dr["Department"]?.ToString(),
                                           Material = dr["Material"]?.ToString(),
                                           Quantity = (int)dr["Quantity"],
                                           FOB = (Decimal)dr["FOB"],
                                           PurchaseDocumentNo = (int)dr["PurchaseDocumentNo"],
                                           Item1 = (int)dr["Item1"],
                                           SAPSO = dr["SAPSO"]?.ToString(),
                                           Item2 = (int)dr["Item2"],
                                           SAPDO = dr["SAPDO"]?.ToString(),
                                           PInt = (int)dr["PInt"],
                                           SLoc = dr["SLoc"]?.ToString(),
                                           Temp1 = dr["Temp1"]?.ToString(),
                                           Seq = (int)dr["Seq"],
                                           Del = dr["Del"]?.ToString(),
                                           Comp = dr["Comp"]?.ToString(),
                                           DeliveryDate = (DateTime)dr["DeliveryDate"],
                                       }).ToList();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                //_tradeLogger.Error("Trading.DAL.Trade.LoadSheet", ex);
                throw ex;
            }
            return lstShippingAdvice;
        }

        public List<ViewModel.Shipping> GetShippingDetails()
        {
            List<ViewModel.Shipping> lstShipping = new List<ViewModel.Shipping>();
            // SqlParameter parameter;
            try
            {
                using (SqlConnection connection = new SqlConnection(TradeDBConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Shipping";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataSet tradeAndShippingDetails = new DataSet();
                    adapter.Fill(tradeAndShippingDetails);



                    if (tradeAndShippingDetails.Tables[0].Rows.Count > 0)
                    {
                        lstShipping = (from DataRow dr in tradeAndShippingDetails.Tables[0].Rows
                                       select new ViewModel.Shipping
                                       {
                                           TradeSheetName = dr["TradeSheetName"]?.ToString(),
                                           BLConsignee = dr["BLConsignee"]?.ToString(),
                                           FinalDestination = dr["FinalDestination"]?.ToString(),
                                           Freight = dr["Freight"]?.ToString(),
                                           LCExpiryDate = dr["LCExpiryDate"]?.ToString(),
                                           LCIssuanceDate = dr["LCIssuanceDate"]?.ToString(),
                                           LCIssuingBank = dr["LCIssuingBank"]?.ToString(),
                                           LCNo = dr["LCNo"]?.ToString(),
                                           PartialShipment = dr["PartialShipment"]?.ToString(),
                                           PaymentTerms = dr["PaymentTerms"]?.ToString(),
                                           PortOfDischarge = dr["PortOfDischarge"]?.ToString(),
                                           PortOfLoading = dr["PortOfLoading"]?.ToString(),
                                           RequiredBLDate = dr["RequiredBLDate"]?.ToString(),
                                           ShipmentExpiryDate = dr["ShipmentExpiryDate"]?.ToString(),
                                           ShipToParty = dr["ShipToParty"]?.ToString(),
                                           SIDate = dr["SIDate"]?.ToString(),
                                           SINo = dr["SINo"]?.ToString(),
                                           SoldToParty = dr["SoldToParty"]?.ToString(),
                                           TradeTerms = dr["TradeTerms"]?.ToString(),
                                           Transportation = dr["Transportation"]?.ToString(),
                                           TransShipment = dr["TransShipment"]?.ToString(),
                                           Vender = dr["Vender"]?.ToString(),
                                           Via = dr["Via"]?.ToString()
                                       }).ToList();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _tradeLogger.Error("Trading.DAL.Trade.LoadSheet", ex);
                throw ex;
            }
            return lstShipping;
        }
    }
}
