using System;
using System.Collections.Generic;
using System.Text;
using Trading.ViewModel;

namespace Trading.DAO
{
    public class UploadTrade
    {
        public bool IsSINoAvailable{ get; set; } = true;

        public int ShippingId { get; set; }

        public Shipping Shipping { get; set; }

        public System.Collections.Generic.List<DocumentInstruction> DocumentInstructions { get; set; }

        public System.Collections.Generic.List<ShippingModel> ShippingModels { get; set; }
    }
    
    public class UploadTradeLog
    {
        public int? ShippingId { get; set; }

        public string WorkBookName { get; set; }

        public string TradeRequest { get; set; }

        public DateTime ImportDate { get; set; }

        public string ImportStatus { get; set; }

        public string ExceptionMessage { get; set; }
    }
}
