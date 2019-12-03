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
                    parameter.ParameterName = "@ShippingId";
                    parameter.SqlDbType = SqlDbType.Int;
                    parameter.Value = shippingId;
                    cmd.Parameters.Add(parameter);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet tradeAndShippingDetails = new DataSet();
                    adapter.Fill(tradeAndShippingDetails, "Trade");

                    uploadedTrade.ShippingId = shippingId;
                    uploadedTrade.Shipping = (from DataRow dr in tradeAndShippingDetails.Tables[0].Rows
                                              select new Shipping
                                              {
                                                  TradeSheetName = dr["TradeSheetName"].ToString()
                                              }).FirstOrDefault();
                    uploadedTrade.DocumentInstructions = (from DataRow dr in tradeAndShippingDetails.Tables[1].Rows
                                                          select new DocumentInstruction
                                                          {
                                                              Instruction = dr["Instruction"].ToString()
                                                          }).ToList();
                    uploadedTrade.ShippingModels = (from DataRow dr in tradeAndShippingDetails.Tables[2].Rows
                                                    select new ShippingModel
                                                    { 
                                                        PONo = dr["PONo"].ToString()
                                                    }).ToList();
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
