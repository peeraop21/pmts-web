namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IColorAPIRepository
    {
        string GetColorList(string factoryCode, string token);
        string GetColorMaintainList(string factoryCode, string token);

        void SaveColor(string factoryCode, string jsonString, string token);

        void UpdateColor(string factoryCode, string jsonString, string token);

        void DeleteColor(string jsonString, string token);
        string GetColorByShadeAndFactoryCode(string shade, string factoryCode, string token);
    }
}
