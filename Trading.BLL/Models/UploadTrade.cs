using System;
using System.Collections.Generic;
using System.Text;

namespace Trading.BLL.Models
{
    class UploadTrade
    {
        public Shipping Shipping { get; set; }

        public List<DocumentInstruction> DocumentInstructions { get; set; }

        public List<ShippingModel> ShippingModels { get; set; }
    }
}
