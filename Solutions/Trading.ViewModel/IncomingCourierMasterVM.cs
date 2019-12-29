using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.ViewModel
{
    public class IncomingCourierMasterVM : IncomingCourierMasterVMSave
    {
        public List<IncomingCourierDetailsVM> CourierDetails { get; set; }
    }

    public class IncomingCourierMasterVMSave
    {
        public Guid CompanyID { get; set; }
        public int MasterID { get; set; }
        public string AWBNo { get; set; }
        public int CourierCompany { get; set; }
        public int ReceivedFrom { get; set; }
        public int CourierFor { get; set; }
        public int DocumentType { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime HandedOverOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

}