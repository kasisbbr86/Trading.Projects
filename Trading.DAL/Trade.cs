using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Trading.DAO;

namespace Trading.DAL
{
    public class Trade
    {
        private string TradeDBConnectionString { get; set; }

        public Trade()
        {
            // Read from Configuration Manager
            TradeDBConnectionString = "server=localhost; Database=ShippingTrade; Trusted_Connection=True;";
        }

        public void SaveShippingTradeDetails(Shipping shipping, DataTable documentInstructionsTable, DataTable shippingModelsTable)
        {
            SqlParameter parameter;
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

                cmd.ExecuteNonQuery();
            }
        }
    }
}
