namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IProductTypeAPIRepository
    {
        string GetProductTypeList(string factoryCode, string token);

        string GenLV2(string factoryCode, int idProductType, string token);

        string GetProductTypeById(string factoryCode, int Id, string token);

        string GetFormGroupByHierarchyLv2(string factoryCode, string hierarchyLv2, string token);

        string GetProductTypeByBoxType(string Type, string token);

        void CreateProductType(string factoryCode, string jsonString, string token);

        void UpdateProductType(string factoryCode, string jsonString, string token);

        void DeleteProductType(string factoryCode, string jsonString, string token);
        string GetProductTypesByHierarchyLv2s(string factoryCode, string lv2s, string token);


        //tassanai
        string GetFormGroupByHierarchyLv2List(string factoryCode, string hierarchyLv2, string formgroup, string token);
    }
}
