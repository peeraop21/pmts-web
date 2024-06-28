using PMTs.DataAccess.ModelView.MaintenancePaperGrade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenancePaperGradeService
    {
        void GetPaperGrade(MaintenancePaperGradeViewModel maintenancePaperGradeViewModel);
        void SavePaperGrade(MaintenancePaperGradeViewModel maintenancePaperGradeViewModel);
        void UpdatePaperGrade(PaperGradeViewModel PaperGradeViewModel);
    }
}
