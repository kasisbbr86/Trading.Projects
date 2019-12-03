using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Trading.DAO;

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
    }
}
