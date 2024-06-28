using PMTs.DataAccess.ModelView.MaintenanceJoint;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceJointService
    {
        void GetJoint(MaintenanceJointViewModel maintenanceJointViewModel);
        void SaveJoint(MaintenanceJointViewModel maintenanceJointViewModel);
        void UpdateJoint(JointViewModel maintenanceJointViewModel);
    }
}
