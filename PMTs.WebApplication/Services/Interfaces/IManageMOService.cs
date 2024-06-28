using Microsoft.AspNetCore.Http;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.ManageMO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IManageMOService
    {
        void GetMODataBySaleOrder(string saleOrder, ref ManageMOViewModel manageMOViewModel);

        void GetMODatasBySaleOrder(string saleOrderStart, string saleOrderEnd, ref ManageMOViewModel manageMOViewModel);

        void SearchAndCreateNewMODataByMaterialNo(string materialNumber, ref MoDataViewModel moData);

        void UpdateMOData(ref ManageMOViewModel manageMOViewModel, MoData moData, string action);

        void CreateNewMOData(ref ManageMOViewModel manageMOViewModel, MoData moData);

        void SaveAttachFileMOData(string orderItem, List<IFormFile> files, ref ManageMOViewModel manageMOViewModel);

        void DeleteAttachFileMOData(string orderItem, string fileName, string seqNo, ref ManageMOViewModel manageMOViewModel);

        void GetAttachFileMO(string orderItem, ref ManageMOViewModel manageMOViewModel);

        void ImportManualMOFromExcelFile(IFormFile file, ref ManageMOViewModel manageMOViewModel, ref string exceptionMessage);

        int CalculateMoTargetQuantity(string materialNO, string orderQuant, string toleranceOver);

        //Tassanai update
        void GetMOSpec(string orderItem, ref MoSpec moSpec);
        void UpdateMOSpec(string orderItem, string changeInfo);

        //boo update
        EditBlockPlatenModel GetBlockPlatenMaster(string material, string pc);
        EditBlockPlatenModel GetBlockPlatenRouting(string material);
        string UpdateBlockPlatenRouting(List<EditBlockPlatenRouting> model);
        List<SearchTypeModel> GetSearchType();
        void GetMODatasBySearchType(string searchType, string searchText, ref ManageMOViewModel manageMOViewModel);
    }
}
