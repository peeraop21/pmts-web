using Microsoft.AspNetCore.Http;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.Report;
using PMTs.WebApplication.Controllers;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMasterCardService
    {
        MasterCardMO GetMasterCardMO(string Orderitem, bool isPreview, BasePrintMastercardData basePrintMastercard);

        List<MoRouting> GetMoRouting(string _OrderItem);

        MasterCardMO GetMasterCard(string MaterialNo, BasePrintMastercardData basePrintMastercard);

        List<Routing> GetRouting(string MaterialNo);

        void SearchMasterCardMOByKeySearch(ref List<MasterDataRoutingModel> masterDataRoutings, string stratSO, string endSO);

        bool UpdatePublicPrinted(string orderItem);

        Task<PrintMastercardViewModel> SetMasterCardMOFromFile(List<IFormFile> fileUpload);

        MasterCardMO GetMasterCardProductCatalog(string MaterialNo, string Factorycode);

        Task<BasePrintMastercardData> GetBaseOfPrintMastercardMO(BasePrintMastercardData basePrintMastercard, List<string> orderItem, UserTIP userTIP);

        void GetBaseOfPrintMastercard(ref BasePrintMastercardData basePrintMastercard, List<string> materialNo);

        Task<byte[]> SavePDFWithOutAttachFile(ReportController reportController, PrintMasterCardMOModel printMasterCardMOModel);

        List<MoRouting> CheckMORoutingAttachFile(List<string> orderItem);

        byte[] SaveTextFileWithOutAttachFile(ReportController reportController, List<MoRouting> moRoutings);

        Task<byte[]> SavePDFTagFile(ReportController reportController, PrintMasterCardMOModel printMasterCardMOModel);

        List<MasterDataRoutingModel> SearchMasterCardMOByKeySearch(string startSO, string endSO);

        void UpdateMaterCardPrintedByOrderItems(List<string> orderItem);
    }
}