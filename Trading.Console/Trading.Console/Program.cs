using System;
using System.Configuration;
using System.IO;
using Trading.BLL;

namespace Trading.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string tradeFilesWatchFolder = ConfigurationManager.AppSettings["TradeFilesWatchFolder"];
            foreach (var excelFile in Directory.GetFiles(tradeFilesWatchFolder, "*.xlsx"))
            {
                using (Trade tradeSheet = new Trade())
                {
                    var tradeCostsSheet = tradeSheet.LoadSheet(excelFile, 0);
                    if (tradeSheet.ValidateSheet(tradeCostsSheet))
                    {
                        tradeSheet.ProcessSheet(tradeCostsSheet);
                    }
                }
            }

            System.Console.ReadKey();
        }
    }
}
