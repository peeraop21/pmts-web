using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMTs.DataAccess.ModelView.MaintenancePrintMethod
{
    public class MaintenancePrintMethodViewModel
    {
        public IEnumerable<PrintMethodViewModel> PrintMethodViewModelList { get; set; }
        public PrintMethodViewModel PrintMethodViewModel { get; set; }
    }

    public class PrintMethodViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Method { get; set; }
        [Required]
        public int? AmountColor { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }


}
