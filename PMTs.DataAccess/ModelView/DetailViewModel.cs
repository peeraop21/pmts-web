namespace PMTs.DataAccess.ModelView
{
    public class DetailViewModel
    {
        public string SaleORG { get; set; }
        public string PlantCode { get; set; }
        public string EvertFlag { get; set; }
        public string MaterialNo { get; set; }


        //Nut
        public ViewCategories modelCategories { get; set; }
        public ProductSpecViewModel modelProductSpec { get; set; }


        // mon 
        public ProductPropViewModel modelProductProp { get; set; }
        public ProductPropChangeHisViewModel modelProductPropChangeHis { get; set; }
        public ProductERPPlantViewModel modelProductERP { get; set; }
        public ProductERPSaleViewModel modelProductERPSale { get; set; }
        public ProductERPPurchaseViewModel modelProductERPPurchase { get; set; }

        //  public ProductChangeHisViewModel modelProductChangeHis { get; set; }



        //mook
        public ProductCustomer modelProductCustomer { get; set; }
        public ProductInfoView modelProductInfo { get; set; }


        //mod
        public RoutingViewModel modelRouting { get; set; }
        public ProductPictureView modelProductPicture { get; set; }
    }
}
