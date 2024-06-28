using Microsoft.AspNetCore.Http;
using PMTs.DataAccess;
using PMTs.DataAccess.ModelPresale;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IProductInfoService
    {
        void GetProductInfo(NewProductController newProductController, ref TransactionDataModel transactionDataModel);
        void SaveProductInfo(ref TransactionDataModel transactionDataModel, ref bool isOverBackward, ref bool isExistCost);
        bool DescriptionCheck(string description);
        bool ProdCodeCheck(string ProdCode);
        string GenMatNo(string matCode, string factoryCode);
    }
}
