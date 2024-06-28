namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMasterDataAPIRepository
    {
        string GetMasterDataByMaterialNo(string factoryCode, string MaterialNo, string token);

        string GetMasterDatasByMaterialNo(string factoryCode, string MaterialNo, string token);

        string GetMasterDataByMaterialNoAndFactory(string factoryCode, string MaterialNo, string token);

        string GetMasterDataByMaterialNumberNonX(string factoryCode, string MaterialNo, string token);

        string GetMasterDataByMaterialNumberNonNotX(string factoryCode, string MaterialNo, string token);

        string GetMasterDataByBomChild(string factoryCode, string MaterialNo, string Custcode, string ProductCode, string token);

        string GetMasterDataByDescription(string factoryCode, string description, string token);

        // string GetMasterDataList(string factoryCode);

        void SaveMasterData(string jsonString, string token);

        void UpdateMasterData(string jsonString, string token);

        void DeleteMasterData(string jsonString, string token);

        string GetMasterDataByProdCode(string factoryCode, string prodCode, string token);

        string GetMasterDataManufacturing(string factoryCode, string saleOrg, string token);

        string SearchBomStructsByMaterialNo(string factoryCode, string MaterialNo, string token);

        string SearchBomStructsBytxtSearch(string factoryCode, string txtSearch, string token);

        string SearchMasterDatasByMaterialNo(string factoryCode, string MaterialNo, string token);

        //string GetMasterDataByKeySearch(string factoryCode, string typeSearch, string keySearch);

        // string GetMasterDataAllByKeySearch(string keySearch);
        void UpdateMasterDataPDISStatus(string FactoryCode, string MaterialNo, string Status, string token);

        string GetMasterDataTop100Update(string factoryCode, string token);

        string GetMasterDataByMaterialNoOnly(string factoryCode, string materialNo, string token);

        void UpdateMasterDataTransStatusByBomStruct(string FactoryCode, string MaterialNo, string Status, string token);

        void UpdateCapImgTransactionDetail(string FactoryCode, string MaterialNo, string Status, string token);

        string GetMasterDataByMaterialNumberX(string factoryCode, string originalMaterialNo, string token);

        string GetMasterProductCatalog(string factoryCode, string modelProductcatagory, string token);

        string GetMasterProductCatalogNotop(string factoryCode, string modelProductcatagory, string token);

        string GetCountProductCatalogNotop(string factoryCode, string modelProductcatagory, string token);

        string UpdateLotsMasterData(string factoryCode, string user, string flagUpdate, string jsonString, string token);

        string GetMasterDatasByMatSaleOrgNonX(string factoryCode, string materialNo, string token);

        string GetMasterDatasByMaterialNos(string factoryCode, string materialNOs, string token);

        //jwt token
        string GetMasterDataByKeySearch(string factoryCode, string typeSearch, string keySearch, string token, string flag);

        string GetMasterDataList(string factoryCode, string token);

        string GetMasterDataAllByKeySearch(string keySearch, string token);

        string GetReuseMasterDatasByMaterialNos(string factoryCode, string materialNOs, string token);

        string GetReuseMasterDataRoutingsByMaterialNos(string factoryCode, string materialNOs, string token);

        void UpdateReuseMaterialNos(string factoryCode, string parentModel, string token);

        void UpdateProductCodeAndDescriptionFromPresaleNewMat(string factoryCode, string pc, string description, string materialOriginal, string token);

        string CreateChangeBoardNewMaterial(string factoryCode, string user, bool checkImport, string jsonData, string token);

        string GetOutsourceFromMaterialNoAndSaleOrg(string factoryCode, string materialNo, string saleOrg, string factoryCodeOutsource, string token);

        void UpdateRoutingPDISStatusEmployment(string FactoryCode, string MaterialNo, string Status, string SaleOrg, string token);

        string CreateChangeFactoryNewMaterial(string factoryCode, string username, bool checkImport, string jsonData, string token);

        string UpdateMasterDatasFromExcelFile(string factoryCode, string username, string masterdataJson, string token);

        string UpdateRoutingsFromExcelFile(string factoryCode, string username, string routingJson, string token);

        string GetMasterDataAllByKeySearchAddTag(string ddlSearch, string inputSerach, string factoryCode, string token);

        string GetMasterDataByUser(string factoryCode, string user, string token);

        void UpdateMasterDataByChangePalletSize(string factoryCode, string user, string jsonString, string token);

        string GetMasterDataRoutingsByMaterialNos(string factoryCode, string materialNOs, string token);

        string SearchMasterDataByMaterialNo(string factoryCode, string materialNo, string token);

        string GetMasterDataList(string materialNo, string pc, string factoryCode, string token);

        void UpdateMasterDatas(string factoryCode, string v, string token);

        string GetBoardDistinctFromMasterData(string factoryCode, string token);

        string GetForTemplateChangeBoardNewMaterials(string factoryCode, string jsonString, string token);

        string GetCustomerDistinctFromMasterData(string factoryCode, string token);

        string ReportCheckStatusColor(string factoryCode, int colorId, string token);
    }
}