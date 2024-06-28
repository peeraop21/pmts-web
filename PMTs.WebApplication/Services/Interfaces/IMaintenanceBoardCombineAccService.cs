using Microsoft.AspNetCore.Http;
using PMTs.DataAccess.ModelView.MaintenanceBoardCombineAcc;
using PMTs.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceBoardCombineAccService
    {
        void ReadExcelFileToBoardCombineAcc(ref string message, ref string planCodeSelect, List<IFormFile> fileUpload);

        void GetBoardCombineAccDataToExcelFile(string planCodeSelect);

        void GetCompanyProfileSelectList(ref MaintenanceBoardCombineAccViewModel maintenanceBoardCombineAccViewModel);
    }
}
