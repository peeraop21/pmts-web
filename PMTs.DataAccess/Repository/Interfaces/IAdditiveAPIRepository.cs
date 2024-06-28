namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IAdditiveAPIRepository
    {
        string GetAdditiveList(string factoryCode, string token);

        string GetAdditiveByMaterialNo(string MaterialNo, string factoryCode, string token);

        string GetAdditiveById(string factoryCode, int Id, string token);

        void SaveAdditive(string jsonString, string token);

        void UpdateAdditive(string jsonString, string token);

        void DeleteAdditive(string factoryCode, string jsonString, string token);
        void SaveAdditiveManual(string jsonString, string token);
        void UpdateAdditiveManual(string jsonString, string token);
    }
}
