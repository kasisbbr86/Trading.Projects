using System;
using System.Collections.Generic;
using System.Text;

namespace Trading.BLL.Models
{
    public class Shipping
    {
        public string SIDATE { get; set; }
        public string SINO { get; set; }
        public string VENDER { get; set; }
        public string SOLDTOPARTY { get; set; }
        public string SHIPTOPARTY { get; set; }
        public string BLCONSIGNEE { get; set; }
        public string PORTOFDISCHARGE { get; set; }
        public string FINALDESTINATION { get; set; }
        public string VIA { get; set; }
        public string TRADETERMS { get; set; }
        public string PORTOFLOADING { get; set; }
        public string PAYMENTTERMS { get; set; }
        public string LCNO { get; set; }
        public string LCISSUANCEDATE { get; set; }
        public string LCISSUINGBANK { get; set; }
        public string LCEXPIRYDATE { get; set; }
        public string SHIPMENTEXPIRYDATE { get; set; }
        public string REQUIREDBLDATE { get; set; }
        public string FREIGHT { get; set; }
        public string PARTIALSHIPMENT { get; set; }
        public string TRANSSHIPMENT { get; set; }
    }
}
