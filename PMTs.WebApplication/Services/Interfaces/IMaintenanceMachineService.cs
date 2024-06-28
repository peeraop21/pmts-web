using PMTs.DataAccess.ModelView.MaintenanceMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceMachineService
    {
        void GetMachine(MaintenanceMachineViewModel maintenanceMachineViewModel);
        void SaveMachine(MaintenanceMachineViewModel maintenanceMachineViewModel);
        void UpdateMachine(MachineViewModel MachineViewModel);
        void SetMachineStatus(MachineViewModel MachineViewModel);
        void DeleteMachine(int Id);
    }
}
