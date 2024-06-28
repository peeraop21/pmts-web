using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Report;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.NewProduct
{
    public class TransactionDataModel
    {
        public enum NumberOfProgress
        {
            Categories = 1,
            Customer = 2,
            ProductInformation = 3,
            ProductSpec = 4,
            ProductProperties = 5,
            ProductRouting = 6,
            ERPInterface = 7,
            Picture = 8
        };

        public TransactionDataModel()
        {
            modelProductSpec = new ProductSpecViewModel();
        }

        public TransactionStatusStyle TransactionStatus { get; set; }
        public TransactionDetail TransactionDetail { get; set; }
        public string CurrentTransaction { get; set; }
        public string Username { get; set; }
        public string SaleOrg { get; set; }
        public string PlantCode { get; set; }
        public string PlantOs { get; set; }
        public string FactoryCode { get; set; }
        public string EventFlag { get; set; }
        public string RealEventFlag { get; set; }
        public string MaterialNo { get; set; }
        public bool SapStatus { get; set; }
        public string PdisStatus { get; set; }
        public int CountMat { get; set; }
        public int Id { get; set; }
        public string PsmId { get; set; }

        //Nut
        public ViewCategories modelCategories { get; set; }
        public ProductSpecViewModel modelProductSpec { get; set; }

        // mon 
        public ProductPropViewModel modelProductProp { get; set; }
        public ProductPropChangeHisViewModel modelProductPropChangeHis { get; set; }
        public ProductERPPlantViewModel modelProductERP { get; set; }
        public ProductERPSaleViewModel modelProductERPSale { get; set; }
        public ProductERPPurchaseViewModel modelProductERPPurchase { get; set; }
        //Mon update for detail 2/8/2562
        public string KindOfProductGroup { get; set; }
        public string ProcessCostName { get; set; }
        public string KindOfProductName { get; set; }
        public string ProductTypeName { get; set; }
        public string MaterialType { get; set; }
        public string UnitMaterial { get; set; }
        public string SaleUnit { get; set; }

        //Tassanai update 11/03/2020
        public ProductPropStandardPatternName modelStandardPatternName { get; set; }

        //  public ProductChangeHisViewModel modelProductChangeHis { get; set; }

        //mook
        public ProductCustomer modelProductCustomer { get; set; }
        public ProductInfoView modelProductInfo { get; set; }

        //mod
        public RoutingViewModel modelRouting { get; set; }
        public ProductPictureView modelProductPicture { get; set; }

        //  public TransactionsDetail modelTransactionsDetail { get; set; }


        public int? amountColor { get; set; }
        public List<string> modelGroupMachineRemark { get; set; }
        public IEnumerable<BuildRemark> modelBuildRemark { get; set; }


        //====  array routing
        public string arrayPrint { get; set; }
        public string arrayDiecut { get; set; }


        //routing
        public List<MasterCardRouting> Rout { get; set; }

    }

    public class TransactionStatusStyle
    {
        public string Categories { get; set; }
        public string Customer { get; set; }
        public string ProductInformation { get; set; }
        public string ProductSpec { get; set; }
        public string ProductProperties { get; set; }
        public string ProductRouting { get; set; }
        public string ProductERPInterface { get; set; }
        public string ProductPicture { get; set; }
    }

    public class TransactionDetail
    {
        public string ProductTypDetail { get; set; }
        public string HierarchyLV2Detail { get; set; }
        public string CustNameDetail { get; set; }
        public string PCDetail { get; set; }
        public string MaterialDescriptionDetail { get; set; }
        public string BoardDetail { get; set; }
        public string CostDetail { get; set; }
        public string HierarchyDetail { get; set; }
        public int? OrderTypeId { get; set; }
        public bool IsOldMaterial { get; set; }
        public bool IsCreateBOM { get; set; }
        public bool IsOutSource { get; set; } //true = employee: false = employer
        public List<RoutingDataModel> RoutingDetail { get; set; }

        public int MaxStep { get; set; }

        public bool IsPresaleCreateNewMat { get; set; }
        public string MaterialSaleOrg { get; set; }

        //new ppt add by wty 4/8/22
        public string NewPrintPlate { get; set; }
        public string OldPrintPlate { get; set; }
        public string NewBlockDieCut { get; set; }
        public string OldBlockDieCut { get; set; }
        public string ExampleColor { get; set; }
        public string CoatingType { get; set; }
        public string CoatingTypeDesc { get; set; }

        public bool? PaperHorizontal { get; set; }
        public bool? PaperVertical { get; set; }
        public bool? FluteHorizontal { get; set; }
        public bool? FluteVertical { get; set; }
    }

}
