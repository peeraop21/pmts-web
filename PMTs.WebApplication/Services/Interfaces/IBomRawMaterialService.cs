using Microsoft.AspNetCore.Mvc.Rendering;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.BomRawMaterial;
using System.Collections.Generic;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IBomRawMaterialService
    {
        List<SelectListItem> GetListUnitOfMeasureCode();
        List<MasterData> SearchMasterDataByMaterialNo(string materialNo);
        List<PpcRawMaterialMaster> SearchPPCRawMaterialMasterByMaterialNo(string materialNo, string materialDesc);
        List<RawMaterialLineFront> GetRawMaterialProductionBomByFGMaterial(string fgMaterial);
        string CheckMaterialNo(string materialNo);
        void SaveRawMaterialProductionBom(RawMaterialLineRequest model);
        void DeleteRawMaterial(int id);

        List<PpcRawMaterialMaster> SearchPPCRawMaterialMastersByFactoryAndMaterialNoAndDescription(string materialNo, string materialDesc);
        List<PpcRawMaterialMaster> SearchPPCRawMaterialMastersByFactoryCode();
        void SaveRawMaterialMaster(PpcRawMaterialMaster model);
        void UpdateRawMaterialMaster(PpcRawMaterialMaster model);
        void DeleteRawMaterialMaster(int id);
    }
}
