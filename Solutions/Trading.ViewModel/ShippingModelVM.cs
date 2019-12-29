using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.ViewModel
{
  public  class ShippingModelVM
    {
        public string PONo { get; set; }
        public string LineNo { get; set; }

        public string ModelName { get; set; }

        public string Version { get; set; }

        public string Quantity { get; set; }

        public string BLModelName { get; set; }

        public string Description { get; set; }

        public string SCNo { get; set; }
        public string Remarks { get; set; }
    }
}
