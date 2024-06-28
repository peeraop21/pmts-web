using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ModelPresale;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.NewProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IPresaleService
    {
        List<PresaleViewModel> SearchPresale(IConfiguration configuration, PresaleViewModel param);
        MasterDataTransactionModel ImportPresale(IConfiguration configuration, PresaleViewModel presale);
        bool CheckImportPresale(string board, string flute);
        // void UpdatePresale(string psm_id, string materialNo);
        void GetPresaleChangeProduct(ref PresaleChangeViewModel presaleChangeViewModel, string materialNo);
        bool CheckExistMasterData(string facrotyCode, string materialNo);
        void GetCompareProduct(ref PresaleChangeViewModel presaleChangeViewModel, string materialNo, string psmId);

        //void UpdatePresale(string pSM_Id, string materialNo);
        void UpdatePresale(string pSM_Id, string materialNo);

        void ChangeProductPresaleToMaster(string materialNo, string psmId);
        void RejectPresale(string psmId);
        void ApprovePresale(string psmId);
        void DeletePresale(string psmId);
        void GetPresaleChangeProductByKeySearch(ref PresaleChangeViewModel presaleChangeViewModel, string typeSearch, string keySearch);
        List<PresaleViewModel> SearchChangeProductNewMat(IConfiguration configuration, PresaleViewModel presale);
        PresaleMasterData SearchPresaleByPsmId(IConfiguration configuration, string psmId);
        void SentToMasterCardPresale(string status, string psmId);

        void UpdateStatusSameMatFromPmt(string pmtStatus, string matNO);
        void RejectPresaleChangeSameMat(string psmId, int id);
        void ApprovePresaleChangeSameMat(string psmId, int id);
        void UpdateUnHoldToPresale(string materialNo);
        List<PresaleChangeProduct> GetPresaleChangeProductByMaterialNo(string keySearch);
    }
}
