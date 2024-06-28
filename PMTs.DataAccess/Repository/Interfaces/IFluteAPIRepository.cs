namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IFluteAPIRepository
    {
        string GetFluteList(string factoryCode, string token);

        string GetFluteByFlute(string factoryCode, string flute, string token);

        void SaveFlute(string jsonString, string token);

        void UpdateFlute(string jsonString, string token);

        void DeleteFlute(string jsonString, string token);
        string GetFluteMaintain(string factoryCode, string token);
        void AddFluteMaintain(string factoryCode, string jsonString, string token);
        void UpdateFluteMaintain(string factoryCode, string jsonString, string token);
        string GetFlutesAndMachinesByFactoryCode(string factoryCode, string token);
    }
}
