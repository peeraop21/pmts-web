using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenancePlantCostField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenancePlantCostFieldService
    {
        void GetPlantCostField(ref List<PlantCostFieldViewModel> plantCostFields);

        void UpdatePlantCostField(string plantCostFieldArr);
    }
}
