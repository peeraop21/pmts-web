using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMachineFluteTrimAPIRepository
    {
        string AddMachineFluteTrim(string jsonString, string factoryCode, string token);
        string GetDataForInitMachineFluteTrimPage(string factoryCode, string token);
        string GetMachineFluteTrimList(string factoryCode, string token);
        string UpdateMachineFluteTrim(string jsonString, string factoryCode, string token);
    }
}
