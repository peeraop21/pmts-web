using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.ModelView.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface ISaleOrderService
    {
        SaleOrderModel GetMoSpecByOrderItem(string orderitem);
        SaleOrderModel BindMoRouting(string orderitem);
        SaleOrderModel GetMoRoutingByOrderItem(string OrderItem);
        SaleOrderModel GetBuildRemark();
        List<string> GetWeight(string orderitem);
        List<Machine> GetMachineDataByFactorycode(string factorycode);
        RoutingDataModel CalculateRouting(string machineName, string OrderItem);
        List<QualitySpec> GetQualitySpecsByMaterial(string Material);
        string GetMachineGroupByMachine(string machine);
        void MapperAddData(SaleOrderModel sessionModel, SaleOrderModel transactionDataModel, ref RoutingDataModel retuneTransactionModel, ref int seqNum);
        RoutingDataModel MappingModelRoutingUpdateAndDelete(SaleOrderModel sessionModel, SaleOrderModel transactionDataModel);
        SaleOrderModel UpdateRouting(SaleOrderModel model, SaleOrderModel modelToUpdate, RoutingDataModel routingModel);
        SaleOrderModel InsertRouting(SaleOrderModel model, SaleOrderModel transactionDataModel, RoutingDataModel routingModel);
        SaleOrderModel CopyRouting(SaleOrderModel model, int seqNo);
        void SaveMoRouting(SaleOrderModel modelToSave);
        void UpdateRouting(SaleOrderModel modelToSave);
    }
}
