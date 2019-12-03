using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

namespace Trading.BLL
{
    public class Utils
    {
        private static string _servicelogFileNamePrefix = "TradeSheetProcessingService";
        public static void writeLogService(string messageType, string message)
        {
            string logFileFullPath = System.IO.Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _servicelogFileNamePrefix + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
            StreamWriter logStreamWriter;
            if (!File.Exists(logFileFullPath))
            {
                logStreamWriter = File.CreateText(logFileFullPath);
                logStreamWriter.Close();
            }
            using (logStreamWriter = File.AppendText(logFileFullPath))
            {
                logStreamWriter.Write("\r\n");
                logStreamWriter.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + " - TYPE: " + messageType + ", MESSAGE: " + message);
                logStreamWriter.Close();
            }
        }
    }
}
