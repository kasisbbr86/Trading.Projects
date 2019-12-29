using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.ViewModel
{
    public class IncomingCourierDetailsVM : IncomingCourierDetailsVMSave
    {
        public List<IncomingCourierSubDetailsVM> SubDetails { get; set; }
    }

    public class IncomingCourierDetailsVMSave
    {
        public Guid CompanyID { get; set; }
        public int DetailsID { get; set; }
        public int MasterID { get; set; }
        public string SINo { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string DocDetail { get; set; }
        public string RefNo { get; set; }
        public decimal Qty { get; set; }
        public string Remarks { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int ParentDetailsID { get; set; }
        public string ArraySubItem { get; set; }
        public bool IsSubDetail { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }

    public class IncomingCourierSubDetailsVM
    {
        public Guid CompanyID { get; set; }
        public int DetailsID { get; set; }
        public int MasterID { get; set; }
        public string SINo { get; set; }
        public DateTime Date { get; set; }
        public string DocDetail { get; set; }
        public string RefNo { get; set; }
        public decimal Qty { get; set; }
        public string Remarks { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int ParentDetailsID { get; set; }
        public string ArraySubItem { get; set; }
        public bool IsSubDetail { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
