namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IUnitMaterialAPIRepository
    {
        string GetUnitMaterialList(string factoryCode, string token);

        string GetUnitMaterialByName(string Name, string token);

        string GetUnitMaterialById(string factoryCode, int id, string token);
    }

}
