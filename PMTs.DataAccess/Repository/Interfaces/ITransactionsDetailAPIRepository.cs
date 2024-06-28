namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ITransactionsDetailAPIRepository
    {
        string GetTransactionsDetailList(string factoryCode, string token);

        string GetTransactionsDetailByMat(string factoryCode, string mat, string token);

        void SaveTransactionsDetail(string jsonString, string token);

        void UpdateTransactionsDetail(string jsonString, string token);

        void DeleteTransactionsDetail(string jsonString, string token);

        string GetTransactionsDetailsByMaterialNoOnly(string factoryCode, string mat, string token);

        string GetTransactionsDetailFirstOutsource(string factoryCode, string mat, string token);

        string GetTransactionsDetailsByMaterialNOs(string factoryCode, string materialNOs, string token);

        string GetSelectedFirstOutsourceByMaterialNo(string factoryCode, string materialNo, string token);

        string GetAllMatOutsourceByMaterialNo(string factoryCode, string materialNo, string token);

        string GetMatOutsourceByMatSaleOrg(string factoryCode, string materialNo, string token);
    }
}
