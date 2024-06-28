namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IFluteLayerAPIRepository
    {
        string GetFluteLayerList(string factoryCode, string token);

        void SaveFluteLayer(string jsonString, string token);

        void UpdateFluteLayer(string jsonString, string token);

        void DeleteFluteLayer(string jsonString, string token);
    }
}
