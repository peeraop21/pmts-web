using Microsoft.AspNetCore.Http;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.LogisticAndWarehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface ILogisticAndWarehouseService
    {
        TruckOptimizeViewModel SaveAndUpdateTruckOptimize(TruckOptimize truckOptimize);

        IEnumerable<TruckOptimizeViewModel> SearchTruckOptimize(string materialNo, ref bool existMasterData);

        void ImportTruckOptimizeFromFile(IFormFile file, ref TruckOptimizeMainModel result, ref string exceptionMessage);
    }
}
