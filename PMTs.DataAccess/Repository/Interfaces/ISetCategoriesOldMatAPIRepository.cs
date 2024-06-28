namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ISetCategoriesOldMatAPIRepository
    {
        string GetSetCategoriesOldMatList(string factoryCode, string token);

        string GetSetCategoriesOldMatByLV2(string factoryCode, string lv2, string token);

        string GetCategoriesMatrixByLV2(string factoryCode, string lv2, string token);
    }
}
