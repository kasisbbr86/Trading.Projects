using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Trading.ViewModel
{
    public class UploadTradeVM
    {
        public bool IsSINoAvailable { get; set; } = true;

        public int ShippingId { get; set; }

        public Shipping Shipping { get; set; }
        
        public System.Collections.Generic.List<DocumentInstructionVM> DocumentInstructions { get; set; }

        public System.Collections.Generic.List<ShippingModelVM> ShippingModels { get; set; }
    }

    public class Shipping
    {
        public string TradeSheetName { get; set; }
        public string SIDate { get; set; }
        public string SINo { get; set; }
        public string Vender { get; set; }
        public string SoldToParty { get; set; }
        public string ShipToParty { get; set; }
        public string BLConsignee { get; set; }
        public string PortOfDischarge { get; set; }
        public string FinalDestination { get; set; }
        public string Via { get; set; }
        public string Transportation { get; set; }
        public string TradeTerms { get; set; }
        public string PortOfLoading { get; set; }
        public string PaymentTerms { get; set; }
        public string LCNo { get; set; }
        public string LCIssuanceDate { get; set; }
        public string LCIssuingBank { get; set; }
        public string LCExpiryDate { get; set; }
        public string ShipmentExpiryDate { get; set; }
        public string RequiredBLDate { get; set; }
        public string Freight { get; set; }
        public string PartialShipment { get; set; }
        public string TransShipment { get; set; }
    }
}
