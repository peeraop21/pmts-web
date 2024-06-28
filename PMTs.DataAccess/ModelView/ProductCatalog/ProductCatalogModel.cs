using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.ProductCatalog
{
    public class ProductCatalogModel
    {
        public ProductCatalogModel()
        {
            masterDataQuery = new MasterDataQuery();
            masterDataQueries = new List<MasterDataQuery>();
            ProductCatalog = new ProductCatalogs();
            productCatalogs = new List<ProductCatalogs>();
            productCatalogConfigs = new List<ProductCatalogConfig>();
            holdMaterialHistories = new List<HoldMaterialHistory>();
            Plant = new List<Plant>();
            bomviews = new List<MasterData>();
            bomStructView = new List<BomStruct>();
        }
        public MasterDataQuery masterDataQuery { get; set; }
        public List<MasterDataQuery> masterDataQueries { get; set; }
        public List<ProductCatalogs> productCatalogs { get; set; }
        public ProductCatalogs ProductCatalog { get; set; }
        public List<string> columnNames { get; set; }
        public List<string> columnNamesInModal { get; set; }
        public List<ProductCatalogConfig> productCatalogConfigs { get; set; }
        public string RecordCount { get; set; }
        public string Role { get; set; }

        public HoldMaterial holdMaterial { get; set; }
        public List<HoldMaterialHistory> holdMaterialHistories { get; set; }

        public List<Plant> Plant { get; set; }

        public BOMViewModel bomView { get; set; }
        public List<MasterData> bomviews { get; set; }

        public List<BomStruct> bomStructView { get; set; }

    }
}

public class Plant
{
    public string PlantCode { get; set; }
    public string Desc { get; set; }
}
public class PlantModel
{
    public string Plant { get; set; }
    public string PlantsDesc { get; set; }
    public string SaleorgsDesc { get; set; }
}


/// <summary>
/// recieve data by query  in  masterdata
/// </summary>
public class MasterDataQuery
{
    //Master
    public string MaterialNo { get; set; }
    public string FactoryCode { get; set; }
    public string PartNo { get; set; }
    public string Pc { get; set; }
    public string Hierarchy { get; set; }
    public string SaleOrg { get; set; }
    public string Plant { get; set; }
    public string CustCode { get; set; }
    public string CusId { get; set; }
    public string CustName { get; set; }
    public string Description { get; set; }
    public string SaleText { get; set; }
    //public string SaleText2 { get; set; }
    //public string SaleText3 { get; set; }
    //public string SaleText4 { get; set; }
    public string Change { get; set; }
    public string IndGrp { get; set; }
    public string IndDes { get; set; }
    public string MaterialType { get; set; }
    public string PrintMethod { get; set; }
    public string Flute { get; set; }
    public string Board { get; set; }
    public int? Wid { get; set; }
    public int? Leg { get; set; }
    public int? Hig { get; set; }
    public string BoxType { get; set; }
    public string JoinType { get; set; }
    public int? CutSheetLeng { get; set; }
    public int? CutSheetWid { get; set; }
    public int? Bun { get; set; }
    public int? BunLayer { get; set; }
    public int? LayerPalet { get; set; }
    public int? BoxPalet { get; set; }
    public double? WeightBox { get; set; }
    public string SaleUom { get; set; }
    public string BomUom { get; set; }
    public string PalletSize { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string PurTxt { get; set; }
    //public string PurTxt2 { get; set; }
    //public string PurTxt3 { get; set; }
    //public string PurTxt4 { get; set; }
    public string HighGroup { get; set; }
    public string HighValue { get; set; }
    public string PdisStatus { get; set; }

    //PricingMaster
    public DateTime ValidityStartFrom { get; set; }
    public DateTime ValidityEndDate { get; set; }
    public string ConditionNumber { get; set; }
    public double? Rate { get; set; }
    public string CurrencyKey { get; set; }


    //Info Record
    public string Vendorname { get; set; }
    public decimal? Netprice { get; set; }
    public string Unitofnetprice { get; set; }
    public DateTime? SourcelistValidfrom { get; set; }
    public DateTime? SourcelistValidto { get; set; }

    //remark
    //remark
    public string Remark { get; set; }
    public int? Nonmove { get; set; }
    public DateTime? NonMoveMonth { get; set; }
    public int? StockWIP { get; set; }
    public int? StockFG { get; set; }
    public int? StockQA { get; set; }

    public string Hold { get; set; }
    public int? PieceSet { get; set; }
    public string HoldRemark { get; set; }

    // add show col 23/04/2020
    public string CustInvType { get; set; }
    public string CIPInvType { get; set; }
    public string BlockNo { get; set; }
    public string PlateNo { get; set; }
}

/// <summary>
/// merge data by query masterdata and routing privot  data show in html
/// </summary>
public class ProductCatalogs
{
    public string Remark { get; set; }
    public string Pc { get; set; }
    //Master
    public string MaterialNo { get; set; }
    public string FactoryCode { get; set; }
    public string PartNo { get; set; }

    public string Hierarchy { get; set; }
    public string SaleOrg { get; set; }
    public string Plant { get; set; }
    public string CustCode { get; set; }
    public string CusId { get; set; }
    public string CustName { get; set; }
    public string Description { get; set; }
    public string SaleText { get; set; }
    //public string SaleText2 { get; set; }
    //public string SaleText3 { get; set; }
    //public string SaleText4 { get; set; }
    public string Change { get; set; }
    public string IndGrp { get; set; }
    public string IndDes { get; set; }
    public string MaterialType { get; set; }
    public string PrintMethod { get; set; }
    public string Flute { get; set; }
    public string Board { get; set; }
    public int? Wid { get; set; }
    public int? Leg { get; set; }
    public int? Hig { get; set; }
    public string BoxType { get; set; }
    public string JoinType { get; set; }
    public int? CutSheetLeng { get; set; }
    public int? CutSheetWid { get; set; }
    public int? Bun { get; set; }
    public int? BunLayer { get; set; }
    public int? LayerPalet { get; set; }
    public int? BoxPalet { get; set; }
    public double? WeightBox { get; set; }
    public string SaleUom { get; set; }
    public string BomUom { get; set; }
    public string PalletSize { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string PurTxt { get; set; }
    //public string PurTxt2 { get; set; }
    //public string PurTxt3 { get; set; }
    //public string PurTxt4 { get; set; }
    public string HighGroup { get; set; }
    public string HighValue { get; set; }
    public string PdisStatus { get; set; }

    //PricingMaster
    public DateTime? ValidityStartFrom { get; set; }
    public DateTime? ValidityEndDate { get; set; }
    public string ConditionNumber { get; set; }
    public double? Rate { get; set; }
    public string CurrencyKey { get; set; }


    //Info Record
    public string Vendorname { get; set; }
    public decimal? Netprice { get; set; }
    public string Unitofnetprice { get; set; }
    public DateTime? SourcelistValidfrom { get; set; }
    public DateTime? SourcelistValidto { get; set; }

    //remark
    //remark

    public int? Nonmove { get; set; }
    public DateTime? NonMoveMonth { get; set; }
    public int? StockWIP { get; set; }
    public int? StockFG { get; set; }
    public int? StockQA { get; set; }

    //Routing
    public string Machine { get; set; }
    public string Alternative1 { get; set; }
    public string PlateNo { get; set; }
    public string MylaNo { get; set; }
    public string Color { get; set; }
    public string TearTape { get; set; }
    public string BlockNo { get; set; }
    public string RemarkInprocess { get; set; }
    public string BlockNo2 { get; set; }
    public string BlockNo3 { get; set; }
    public string BlockNo4 { get; set; }
    public string BlockNo5 { get; set; }

    //add new 
    public int? NoOpenIn { get; set; }

    public string Hold { get; set; }
    public int? PieceSet { get; set; }
    public string HoldRemark { get; set; }

    // add show col 23/04/2020
    public string CustInvType { get; set; }
    public string CIPInvType { get; set; }

}

/// <summary>
///  recieve data by html call search function
/// </summary>
public class ProductCatalogsSearch
{
    public string Role { get; set; } // 4 = Sale
    public string FactoryCode { get; set; }
    public string pc1 { get; set; }
    public string pc2 { get; set; }
    public string pc3 { get; set; }
    public string pc4 { get; set; }
    public string pc5 { get; set; }
    public string pc6 { get; set; }
    public string partNo { get; set; }
    public string custCode { get; set; }
    public string custID { get; set; }
    public string custName { get; set; }
    public string desc { get; set; }
    public string saleText { get; set; }
    public double wid { get; set; }
    public double leg { get; set; }
    public double hig { get; set; }
    public string flute { get; set; }
    public string board { get; set; }
    public double rate { get; set; }
    public string remark { get; set; }
    public bool idNonMove { get; set; }
    public bool idStockWIP { get; set; }
    public bool idStockFG { get; set; }
    public bool idStockQA { get; set; }
    public bool idHoldFind { get; set; }
    public bool isXPC { get; set; }
    public string blockNo { get; set; }
    public string plateNo { get; set; }
    public string MaterialNo { get; set; }

    // add search col 23/04/2020
    public string CustInvType { get; set; }
    public string CIPInvType { get; set; }

    /// <summary>
    /// prod saleorg
    /// </summary>
    public string FactoryCodeProduction { get; set; }
}



public class ProductCatalogRemark
{
    public string PC { get; set; }
    public string MaterialNo { get; set; }
    public string Remark { get; set; }
    public string NonMoveMonth { get; set; }
    public int? NonMove { get; set; }
    public int? StockWIP { get; set; }
    public int? StockFG { get; set; }
    public int? StockQA { get; set; }

    public string UpdateBy { get; set; }
    public DateTime? LastUpdate { get; set; }

}
