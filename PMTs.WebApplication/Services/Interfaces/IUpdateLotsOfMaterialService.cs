using Microsoft.AspNetCore.Http;
using PMTs.DataAccess.ModelView.UpdateLotsOfMaterial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IUpdateLotsOfMaterialService
    {
        void ReadExcelFileToUpdateMasterData(ref string message, string planCodeSelect, List<IFormFile> fileUpload, ref UpdateLotsOfMaterialViewModel updateLotsOfMaterialViewModel);

        void GetMasterDataToExcelFile(string planCodeSelect);

        void GetCompanyProfileSelectList(ref UpdateLotsOfMaterialViewModel UpdateLotsOfMaterialViewModel);

        void ReadExcelFileToUpdateStatusX(ref string message, string planCodeSelect, List<IFormFile> fileUpload, ref UpdateLotsOfMaterialViewModel updateLotsOfMaterialViewModel);
    }
}
