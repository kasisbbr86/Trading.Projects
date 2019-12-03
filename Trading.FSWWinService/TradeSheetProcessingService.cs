using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trading.BLL;

namespace Trading.FSWWinService
{
    public partial class TradeSheetProcessingService : ServiceBase
    {
        public TradeSheetProcessingService()
        {
            InitializeComponent();
            tradeSheetSystemWatcher.Created += tradeSheetSystemWatcher_Created;
        }

        public string TradeSheetWatcherPath
        {
            get { return ConfigurationManager.AppSettings["TradeSheetWatcherPath-WatchFolder"]; }
        }

        public string TradeSheetProcessedPath
        {
            get { return ConfigurationManager.AppSettings["TradeSheetWatcherPath-Processed"]; }
        }

        public string TradeSheetFailedPath
        {
            get { return ConfigurationManager.AppSettings["TradeSheetWatcherPath-Failed"]; }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                tradeSheetSystemWatcher.Path = TradeSheetWatcherPath;
                Utils.writeLogService("Information", ServiceName + ", Service Started");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void OnStop()
        {
            try
            {
                Utils.writeLogService("Information", ServiceName + ", Service Stopped");
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }

        /// <summary>  
        /// This event monitor folder wheater file created or not.  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        void tradeSheetSystemWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            try
            {
                Thread.Sleep(2000);
                using (Trade tradeSheet = new Trade())
                {
                    tradeSheet.TradeDBConnectionString = ConfigurationManager.ConnectionStrings["TradeConnectionString"].ConnectionString;
                    var tradeCostsSheet = tradeSheet.LoadSheet(Path.Combine(TradeSheetWatcherPath, e.Name), 0);
                    if (tradeSheet.ValidateSheet(tradeCostsSheet))
                    {
                        tradeSheet.ProcessSheet(tradeCostsSheet);
                    }
                }
                File.Move(Path.Combine(TradeSheetWatcherPath, e.Name), Path.Combine(TradeSheetProcessedPath, e.Name));
            }
            catch (Exception ex)
            {
                File.Move(Path.Combine(TradeSheetWatcherPath, e.Name), Path.Combine(TradeSheetFailedPath, e.Name));
                throw ex;
            }
        }
    }
}
