namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ISalesViewAPIRepository
    {
        string GetSalesViewList(string factoryCode, string token);

        string GetSaleViewByMaterialNo(string factoryCode, string materialNo, string token);

        string GetSaleViewsByMaterialNo(string factoryCode, string materialNo, string token);

        string GetSaleViewBySaleOrg(string factoryCode, string materialNo, string pDISStatus, string token);

        string GetSaleViewsByMaterialNoAndFactoryCode(string factoryCode, string materialNo, string token);

        void SaveSaleView(string jsonString, string token);

        void UpdateSaleView(string jsonString, string token);

        void DeleteSaleView(string jsonString, string token);

        string GetSaleViewByMaterialNoAndChannelAndDevPlant(string factoryCode, string materialNo, int channel, string devPlant, string token);

        void DeleteSaleViews(string jsonExistOsSaleViews, string token);

        string GetSaleViewBySaleOrgChannel(string factoryCode, string materialNo, string saleorg, byte channel, string token);

        string GetReuseSaleViewsByMaterialNos(string factoryCode, string materialNos, string token);
    }
}
