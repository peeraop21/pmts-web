using Microsoft.AspNetCore.Http;
using PMTs.DataAccess;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IProductERPService
    {
        void GetProductERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel);

        //plant view
        void SavePlantViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel);
        //void UpdatePlantViewERP(NewProductController newProductController, ref TransactionDataModel model, string btnUpdateERP);
        void UpdatePlantViewERP(NewProductController newProductController, ref TransactionDataModel model);
        void DeletePlantViewERP(NewProductController newProductController, ref TransactionDataModel model, int selectedTab, string btnDeleteERP);

        //sale view
        void SaveSaleViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel);
        void UpdateSaleViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel);
        void DeleteSaleViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel, int selectedTab);

        //purchase view
        void SavePurchaseViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel);

        void GetProductERPData(ref TransactionDataModel model);
        void GetPlantViewData(ref TransactionDataModel PlantData);
        void GetSaleViewData(ref TransactionDataModel SaleData);
        void GetPurchaseViewData(ref TransactionDataModel PurchaseData);

        bool ChkPlantMat(TransactionDataModel PlantData, string btnSave);
        bool ChkSaleMat(TransactionDataModel SaleData, string btnSave);
        string GenPurch(string Plant);

        TransactionDataModel SavePlantViewData(TransactionDataModel PlantData, string btnSave);
        //void UpdatePlantViewData(ref TransactionDataModel PlantData, String UpdateBtn);
        void UpdatePlantViewData(ref TransactionDataModel PlantData);
        TransactionDataModel DeletePlantViewData(TransactionDataModel model, String btnDeleteERP);

        TransactionDataModel SaveSaleViewData(TransactionDataModel SaleData, string btnSave);
        TransactionDataModel UpdateSaleViewData(TransactionDataModel SaleData, String UpdateBtn);
        TransactionDataModel DeleteSaleViewData(TransactionDataModel SaleData, String UpdateBtn);

        TransactionDataModel SavePurchaseViewData(TransactionDataModel PurData);

    }
}

