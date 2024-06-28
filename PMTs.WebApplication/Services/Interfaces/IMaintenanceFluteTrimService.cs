using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceFluteTrim;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceFluteTrimService
    {
        MaintenanceFluteTrimModel InitalPage();
        bool SaveMachineFluteTrim(MachineFluteTrim machineFluteTrim);
        bool UpdateMachineFluteTrim(MachineFluteTrim machineFluteTrim);
    }
}
