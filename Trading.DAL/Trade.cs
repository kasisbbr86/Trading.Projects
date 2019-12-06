using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Trading.DAO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trading.DAL
{
    public class Trade
    {
        public string TradeDBConnectionString { get; set; }

        public Trade()
        {
        }

        public int SaveShippingTradeDetails(Shipping shipping, DataTable documentInstructionsTable, DataTable shippingModelsTable)
        {
            SqlParameter parameter;
            int shippingId = -1;
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
            
            return shippingId;
        }

        public void WriteShippingImportLog(UploadTradeLog uploadTradeLog)
        {
            SqlParameter parameter;
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

        public UploadTrade GetShippingTradeDetails(int shippingId)
        {
            UploadTrade uploadedTrade = new UploadTrade();
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

                    uploadedTrade = new UploadTrade()
                    {
                        Shipping = new Shipping(),
                        DocumentInstructions = new List<DocumentInstruction>(),
                        ShippingModels = new List<ShippingModel>()
                    };
                    uploadedTrade.ShippingId = shippingId;
                    if (tradeAndShippingDetails.Tables[0].Rows.Count > 0)
                    {
                        uploadedTrade.Shipping = (from DataRow dr in tradeAndShippingDetails.Tables[0].Rows
                                                  select new Shipping
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
                                                              select new DocumentInstruction
                                                              {
                                                                  Instruction = dr["Instruction"]?.ToString()
                                                              }).ToList();
                    }

                    if (tradeAndShippingDetails.Tables[2].Rows.Count > 0)
                    {
                        uploadedTrade.ShippingModels = (from DataRow dr in tradeAndShippingDetails.Tables[2].Rows
                                                        select new ShippingModel
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
                throw ex;
            }
            return uploadedTrade;
        }
    }
}
