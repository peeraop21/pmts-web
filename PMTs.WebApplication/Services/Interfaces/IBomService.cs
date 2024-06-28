using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IBomService
    {
        void GetMatParent(ref BOMViewModel bomViewModel, string txtSearch, string ddlSearch);
        BOMViewModel GetMatChild(BOMViewModel model);
        BOMViewModel GetBomStruct(string materialNo);
        void SaveBomStruct(BOMViewModel model);
        string DeleteBomStruct(int Id);
        void GetParentChildByMat(ref BOMViewModel bomViewModel, string Mat);
        void GetMatParentToProductCatalog(ref BOMViewModel bomViewModel, string txtSearch, string ddlSearch, string FactoryCode);
        void EditBOMStruct(BomStruct bomStruct);
        List<Plants> GetAllPlant();
        string GetAllGroupCompany();
        void CopyMatNewPlant(string parentmat, string plants);
        MasterData GetMasterdataByMaterial(string parentmat, string plants);
        void ResentBOM(string materialNo);
    }
}
