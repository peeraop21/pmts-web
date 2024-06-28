namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMachineAPIRepository
    {
        string GetMachineList(string factoryCode, string token);

        string GetMachineGroupByMachine(string factoryCode, string machine, string token);

        string GetMachineGroupByPlanCode(string factoryCode, string planCode, string token);

        string GetMachineById(int Id, string token);

        void SaveMachine(string jsonString, string token);

        void UpdateMachine(string jsonString, string token);

        void DeleteMachine(string jsonString, string token);

        string GetMachinesByPlanCodes(string factoryCode, string saleOrders, string token);


        //Tassanai 14122021 AutoRouting
        string GetMachineByMachineGroup(string factoryCode, string machineGroup, string token);

        string GetMachineHierarchy(string factoryCode, string hieLv2, string MaterialNo, string floxotype, string JoinType, string token);

        // string GetMachineAutorouting(string factoryCode, string machineGroup, string token);

    }
}
