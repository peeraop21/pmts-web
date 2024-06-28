using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.DataAccess.ModelView.MaintenanceFluteTrim
{
    public class MaintenanceFluteTrimModel
    {
        public List<MachineFluteTrim> MachineFluteTrims { get; set; }   
        public List<string> MachineOptions { get; set; }
        public List<string> FluteOptions { get; set; }
    }
}
