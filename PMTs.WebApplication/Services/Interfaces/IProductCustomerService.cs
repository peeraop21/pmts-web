using Microsoft.AspNetCore.Http;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IProductCustomerService
    {
        void GetCustomer(NewProductController newProductController, ref TransactionDataModel transactionDataModel);
        void SaveCustomer(ref TransactionDataModel transactionDataModel, string QaSpecArr);
        TransactionDataModel CustomerData(TransactionDataModel transactionDataModel);
    }
}
