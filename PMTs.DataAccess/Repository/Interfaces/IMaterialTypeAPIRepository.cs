namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMaterialTypeAPIRepository
    {
        string GetMaterialTypeList(string token);
        string GetMaterialTypeByMaterialCode(string matCode, string token);
        string GetMaterialCode(int Id, string token);
    }
}
