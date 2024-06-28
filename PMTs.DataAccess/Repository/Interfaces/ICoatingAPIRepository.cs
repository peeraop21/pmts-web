namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ICoatingAPIRepository
    {
        string GetCoatingList(string factoryCode, string token);

        string GetCoatingByMaterialNo(string factoryCode, string MaterialNo, string token);

        string GetCoatingById(string factoryCode, int Id, string token);

        void SaveCoating(string Factorycode, string jsonString, string token);

        void UpdateCoating(string jsonString, string token);

        void DeleteCoating(string factoryCode, string MaterialCode, string token);
    }
}
