using Microsoft.AspNetCore.Http;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.AddTagCustomer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMasterDataService
    {
        void GetMasterData(ref List<MasterDataRoutingModel> masterDataRoutingModel, string typeSearch, string keySearch, string flag, bool isMaterialOnly);
        void DeleteMasterData(string Material);
        void UpdateReuseMaterialNos(ref List<MasterData> masterDatas, List<string> materialNOs);
        void GetChangePalletSizeData(ref ChangePalletSizeViewModel changePalletSize, string materialNo);
        void GetReUseMasterDatas(ref List<MasterDataRoutingModel> masterDatas, string materialNo, List<string> materialNos);
        void SaveAttachFileMO(IFormFile formFile);
        void DeleteAttachFileMO();
        MemoryStream GetAttachFilePDFMOFromMasterData(string materialNo);
        MemoryStream GetSemiFilePDFMOFromMasterData(string materialNo);
        void SaveAttachFileSemi(IFormFile formFile);
        void DeleteAttachFileSemi();
        void ImportUpdateMaterialFromFile(IFormFile file, ref string exceptionMessage);
        //VMasterDataViewModel GetMasterDataListView();
        //VMasterDataViewModel GetMasterDataListViewSearch(string TxtSearch, string ddlSearch);

        void GetMasterDataAddTag(ref MaintainAddTagCustomerModel addTagCustomerModel, string ddlsearch, string inputSerach);
        void UpdateTagMaterial(ref AddTagCustomerModel addTagCustomerModel);
        void UpdateMasterDataByChangePalletSize(MasterData masterData);
        Task<List<MasterDataRoutingModel>> GetMasterDataFromFile(List<IFormFile> fileUpload);
        List<MasterData> GetMasterDataList(string MaterialNo, string PC);
        void ReSentMat(string materialno);
    }
}
