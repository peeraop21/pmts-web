using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.MaintenanceAllowance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceAllowanceService
    {
        void GetAllowance(MaintenanceAllowanceViewModel maintenanceAllowanceViewModel);
        void SaveAllowance(MaintenanceAllowanceViewModel maintenanceAllowanceViewModel);
        void UpdateAllowance(AllowanceViewModel AllowanceViewModel);
        void DeleteAllowance(int Id);
    }
}
