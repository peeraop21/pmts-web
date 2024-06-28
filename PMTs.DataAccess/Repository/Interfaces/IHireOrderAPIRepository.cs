namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IHireOrderAPIRepository
    {
        string GetAllHireOrder(string factoryCode, string token);
        string GetHireOrderById(string factoryCode, int id, string token);
    }
}
