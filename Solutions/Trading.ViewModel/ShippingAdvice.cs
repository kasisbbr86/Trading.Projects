using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.ViewModel
{
    public class ShippingAdviceVM
    {
        public Guid CompanyID { get; set; }
        public int ShippingAdviceID { get; set; }
        public string SCInvoiceNo { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string Consignee { get; set; }
        public DateTime BLDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Shiper { get; set; }
        public string BLNo { get; set; }
        public string Factory { get; set; }
        public string Department { get; set; }
        public string Material { get; set; }
        public int Quantity { get; set; }
        public decimal FOB { get; set; }
        public int PurchaseDocumentNo { get; set; }
        public int Item1 { get; set; }
        public string SAPSO { get; set; }
        public int Item2 { get; set; }
        public string SAPDO { get; set; }
        public int PInt { get; set; }
        public string SLoc { get; set; }
        public string Temp1 { get; set; }
        public int Seq { get; set; }
        public string Del { get; set; }
        public string Comp { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}
