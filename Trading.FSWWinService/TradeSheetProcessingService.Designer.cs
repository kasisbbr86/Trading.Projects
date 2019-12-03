using System.ServiceProcess;

namespace Trading.FSWWinService
{
    partial class TradeSheetProcessingService : ServiceBase
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tradeSheetSystemWatcher = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this.tradeSheetSystemWatcher)).BeginInit();
            // 
            // tradeSheetSystemWatcher
            // 
            this.tradeSheetSystemWatcher.EnableRaisingEvents = true;
            this.tradeSheetSystemWatcher.Filter = "*.xlsx";
            this.tradeSheetSystemWatcher.IncludeSubdirectories = true;
            // 
            // TradeSheetProcessingService
            // 
            this.ServiceName = "TradeSheetProcessingService";
            ((System.ComponentModel.ISupportInitialize)(this.tradeSheetSystemWatcher)).EndInit();

        }

        #endregion

        private System.IO.FileSystemWatcher tradeSheetSystemWatcher;
    }
}
