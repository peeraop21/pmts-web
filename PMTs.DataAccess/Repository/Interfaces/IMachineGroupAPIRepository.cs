namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMachineGroupAPIRepository
    {
        string GetMachineGroupList(string factoryCode, string token);

        string GetMachineGroupByMaterialNo(string MaterialNo, string factoryCode, string token);

        string GetMachineGroupById(string factoryCode, int Id, string token);

        void SaveMachineGroup(string jsonString, string token);

        void UpdateMachineGroup(string jsonString, string token);

        void DeleteMachineGroup(string factoryCode, string jsonString, string token);
        string GetByMachineGroupJoinMachine(string factoryCode, string token);
    }
}
