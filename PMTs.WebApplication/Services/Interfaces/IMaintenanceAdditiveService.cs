using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceAdditive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceAdditiveService
    {
        MaintenanceAdditiveViewModel GetAdditive();
        void AddAdditive(Additive additive);
        void UpdateAdditive(Additive additive);
    }
}
