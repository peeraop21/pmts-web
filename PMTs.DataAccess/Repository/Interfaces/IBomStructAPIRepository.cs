namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IBomStructAPIRepository
    {
        string GetBomStructList(string factoryCode, string token);

        string GetBomStructById(string factoryCode, int Id, string token);

        string SearchBomStructByMaterialNo(string factoryCode, string MaterialNo, string token);

        void SaveBomStruct(string factoryCode, string jsonString, string token);

        void UpdateBomStruct(string factoryCode, string jsonString, string token);

        void DeleteBomStruct(string jsonString, string token);
        void CopyBomStrucToNewPlant(string parentmat, string plants, string factorycode, string token);

    }
}
