using Microsoft.AspNetCore.Http;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.NewProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IRoutingService
    {
        void BindDataToModel(TransactionDataModel model);
        RoutingDataModel CalculateCorProp();
        RoutingDataModel CalculateRouting(string Machine);
        List<Machine> GetMachineList(string keywordMachine, string machineGroup);
        List<BuildRemark> GetRemarkList(string keywordRemark);
        List<string> GetWeight(TransactionDataModel modelSession);
        string GetMachineGroupByMachine(string machine);
        List<int> GetPlatenRotary(string machine);
        List<Color> GetShadeByInkList(string inkName);
        List<Color> GetInkByShadeList(string shadeName);
        List<Color> GetInkShadeList();
        void SaveRouting(TransactionDataModel model);
        void UpdateRouting(TransactionDataModel model);
        bool GetMachineDataPlatenAndRotalyByMachine(string machine);
        List<RoutingDataModel> GetRoutingList(string MaterialNo);
        List<ScoreGap> GetScoreGapList(string flut, string scoretypeid);
        void InitialPresaleRouting();
        void MappingModelRouting(TransactionDataModel sessionModel, TransactionDataModel transactionDataModel, ref RoutingDataModel retuneTransactionModel, ref int seqNum);
        RoutingDataModel MappingModelRoutingUpdateAndDelete(TransactionDataModel sessionModel, TransactionDataModel transactionDataModel);
        TransactionDataModel UpdateRouting(TransactionDataModel model, TransactionDataModel modelToUpdate, RoutingDataModel routingModel);
        TransactionDataModel InsertRouting(TransactionDataModel model, TransactionDataModel transactionDataModel, RoutingDataModel routingModel);
        TransactionDataModel CopyRouting(TransactionDataModel model, int seqNo);
        List<CorConfig> GetCoreConfig();
        // void UpdatePDISStatus(string MaterialNo, string Status);
        void UpdatePDISStatus(string Factorycode, string MaterialNo, string Status);
        void UpdatePlantViewShipBlk(string MaterialNo, string Status);
        RoutingDataModel CalculateNewPaperRoll(string PaperwWidth, string WidthIn, string Flut, string cut);
        RoutingDataModel CalculateNewCut(string Cut, string WidthIn, string Flut, string materialNo, string machine);
        RoutingDataModel CalculateRouting(string machineName, TransactionDataModel ModelTrans, int wid);
        List<QualitySpec> GetQualitySpecsByMaterial(string Material);
        List<Machine> GetMachineDataByFactorycode(string factorycode);
        void PresaleSaveRouting(TransactionDataModel modelToSave);
        void DeleteRoutingByMaterialNoAndFactoryAndSeq(string Material, string Seq);
        Machine GetMachine(string machine);
        void UpdateRoutingPDISStatusEmployment(string MaterialNo, string Status, string SaleOrg);

        List<RoutingDataModel> AutoRoutingList(TransactionDataModel model);

    }
}
