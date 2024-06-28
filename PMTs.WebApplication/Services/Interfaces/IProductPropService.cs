using Microsoft.AspNetCore.Http;
using PMTs.DataAccess;
using PMTs.DataAccess.ModelView.NewProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IProductPropService
    {
        void ProductPropData(ref TransactionDataModel transactionDataModel);

        //void ProductPropData(TransactionDataModel transactionDataModel);

        // TransactionDataModel ProductProp(TransactionDataModel Propdata);
        // void ProductProp(TransactionDataModel Propdata);

        void SaveProductProp(ref TransactionDataModel temp);

        void GetPallet(ref TransactionDataModel transactionDataModel, string JoinTypeFilter, string palletSizeFilter, int WidDC, int LegDC, int Overhang);

        //TransactionDataModel UpdateChangeHisData(TransactionDataModel temp);
    }
}
