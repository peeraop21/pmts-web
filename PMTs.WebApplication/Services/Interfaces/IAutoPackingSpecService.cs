using Microsoft.AspNetCore.Http;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.AutoPackingSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IAutoPackingSpecService
    {
        AutoPackingSpecViewModel SaveAndUpdateAutoPackingSpec(AutoPackingSpec autoPackingSpec);

        IEnumerable<AutoPackingSpecViewModel> SearchAutoPackingSpec(string materialNo, ref bool existMasterData);

        void ImportAutoPackingSpecFromFile(IFormFile file, ref AutoPackingSpecMainModel result, ref string exceptionMessage);
        void GetAutoPackingConfigs(ref AutoPackingSpecMainModel result);
    }
}
