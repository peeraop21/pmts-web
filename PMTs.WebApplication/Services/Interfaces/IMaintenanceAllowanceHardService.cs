using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceAllowanceHard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceAllowanceHardService
    {
        MaintenanceAllowanceHardViewModel GetAllowancehard();
        void AddAllowanceHard(AllowanceHard allowance);
        void UpdateAllowanceHard(AllowanceHard allowance);

    }
}
