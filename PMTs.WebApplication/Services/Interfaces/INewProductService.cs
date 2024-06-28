using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.NewProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface INewProductService
    {
        MasterDataTransactionModel GetMasterDataTransaction(string actionTran, string materialNo, string factoryCode);
        MasterDataTransactionModel GetMasterDataTransactionPresale(MasterDataTransactionModel oMasterData);
        //TransactionDataModel BindDataToModelEditCopy( IHostingEnvironment hostEnvironment, string actionTran, string MaterialNo, MasterDataTransactionModel masterDataTran);
        ProductERPPurchaseViewModel BindDataPurchase(MasterDataTransactionModel obj);
        ProductERPPurchaseViewModel BindDataPurchaseByMatNo(string materialNo);
        ProductPropViewModel BindProductProp(MasterDataTransactionModel obj);
        ProductCustomer BindCustomerData(MasterDataTransactionModel obj);
        //  RoutingViewModel BindRoutingData(MasterDataTransactionModel obj, ProductSpecViewModel modelProductSpec);
        RoutingViewModel BindRoutingData(MasterDataTransactionModel obj, ProductSpecViewModel modelProductSpec, TransactionDataModel trans);
        ViewCategories BindCategoriesData(MasterDataTransactionModel obj);
        //ViewCategories BindCategoriesPresaleData(string MaterialNo);
        ProductSpecViewModel BindProductSpecData(MasterDataTransactionModel obj, IHostingEnvironment hostEnvironment);
        string GetBoardKIWI(string boardCombine);
        double? GetBasisWeight(string code, string flute, string factoryCode);
        decimal ConvertmmToInch(int? mm);
        ProductInfoView BindProductInfoData(MasterDataTransactionModel obj);
        ProductPictureView BindProductPictureData(MasterDataTransactionModel obj, IHostingEnvironment hostEnvironment);
        TransactionDetail BindDataTransactionDetail(TransactionDataModel model);
        void SetTransactionStatus(ref TransactionDataModel model, string transactionName);
        void GetCategoriesListData(ref TransactionDataModel transactionDataModel);
        void GetCompanyProfiles(ref OutsourcingViewModel outsourcingViewModel);
        void SaveFactoryCodeToSession(OutsourcingViewModel outsourcingViewModel);
        void GetHireOrders(ref OutsourcingViewModel outsourcingViewModel);
        void GetHireMappings(ref OutsourcingViewModel outsourcingViewModel);
        void CheckDuplicateMaterial(string plantOs, string materialNo, string action);
        void UpdateMaxProgress(string factoryCode, ref TransactionDataModel transactionDataModel);
        //string CalSizeDimensions(MasterData masterData, List<Routing> routings);
    }
}

