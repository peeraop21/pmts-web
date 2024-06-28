namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IBuildRemarkAPIRepository
    {
        string GetBuildRemarkList(string factoryCode, string token);

        void SaveBuildRemark(string factoryCode, string jsonString, string token);

        void UpdateBuildRemark(string factoryCode, string jsonString, string token);

        void DeleteBuildRemark(string jsonString, string token);
    }
}
