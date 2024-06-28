using Microsoft.AspNetCore.Http;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceProductInfoService
    {
        void DeleteProductViewList(string materialNo);

        void ReadExcelFileToChangeBoardNewMaterial(ref List<ChangeBoardNewMaterial> modelResult, List<IFormFile> fileUpload, bool checkImport);
        void ReadExcelFileToChangeFactoryNewMaterial(ref List<ChangeBoardNewMaterial> modelResult, List<IFormFile> fileUpload, bool checkImport);
        List<Routing> GetRoutingDataList(string materialNo);
        List<PpcRawMaterialProductionBom> GetBomRawMatDataList(string materialNo);
        void ReplaceRoutingDataList(string MaterialFrom, string MaterialTo);
        void InitialDataChangeBoardNewMaterial(ref List<string> boards, ref List<Customer> customers, ref List<string> grades);
        List<ChangeBoardNewMaterial> GetForTemplateChangeBoardNewMaterials(SearchMaterialTemplateParam param);
    }
}
