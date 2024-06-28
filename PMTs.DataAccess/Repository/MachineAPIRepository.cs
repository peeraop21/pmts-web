using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;
using System.Text;

namespace PMTs.DataAccess.Repository
{
    public class MachineAPIRepository : IMachineAPIRepository
    {
        private readonly string _actionName = "Machine";

        public string GetMachineList(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
        public string GetMachineById(int Id, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMachineById" + "?AppName=" + Globals.AppNameEncrypt + "&id=" + Id, string.Empty, token);
            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        private static string ConvertStringToHex(String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public string GetMachineGroupByMachine(string factoryCode, string machine, string token)
        {

            machine = ConvertStringToHex(machine, System.Text.Encoding.Unicode);
            //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(machine);
            //machine  =  System.Convert.ToBase64String(plainTextBytes);
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetMachineGroupByMachine" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&Machine=" + machine, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMachineGroupByPlanCode(string factoryCode, string planCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetMachineGroupByPlanCode" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&PlanCode=" + planCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void SaveMachine(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateMachine(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteMachine(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMachinesByPlanCodes(string factoryCode, string planCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetMachinesByPlanCodes" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, planCode, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMachineHierarchy(string factoryCode, string hieLv2, string MaterialNo, string floxotype, string JoinType, string token)
        {

            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMachineHierarchy" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&HieLv2=" + hieLv2 + "&MaterialNo=" + MaterialNo + "&Floxotype=" + floxotype + "&JoinType=" + JoinType, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
        public string GetMachineByMachineGroup(string factoryCode, string machineGroup, string token)
        {

            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMachineByMachineGroup" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&machineGroup=" + machineGroup, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

    }
}
