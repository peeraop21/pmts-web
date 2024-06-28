namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IFluteTrAPIRepository
    {
        string GetFluteTrList(string factoryCode, string token);

        string GetFluteTrByFlute(string factoryCode, string flute, string token);

        void SaveFluteTr(string jsonString, string token);

        void UpdateFluteTr(string jsonString, string token);

        void DeleteFluteTr(string jsonString, string token);

        string GetFluteTrsByFlutes(string factoryCode, string flutes, string token);
    }
}
