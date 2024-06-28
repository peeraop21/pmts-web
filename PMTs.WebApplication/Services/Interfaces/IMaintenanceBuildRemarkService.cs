
using PMTs.DataAccess.ModelView.MaintenanceBuildRemark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceBuildRemarkService
    {
        void GetBuildRemark(MaintenanceBuildRemarkViewModel maintenanceBuildRemarkViewModel);
        void SaveBuildRemark(MaintenanceBuildRemarkViewModel maintenanceBuildRemarkViewModel);
        void UpdateBuildRemark(BuildRemarkViewModel BuildRemarkViewModel);
    }
}
