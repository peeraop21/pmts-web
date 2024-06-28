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
    public interface ICategoryService
    {
        void GetCategory(NewProductController newProductController, ref TransactionDataModel transactionDataModel, string actionTran, string materialNo, string psmId);
        void SaveCategories(TransactionDataModel transactionDataModel);
        TransactionDataModel BindDataToModelCategoriesBackward(TransactionDataModel transactionDataModel);
    }
}
