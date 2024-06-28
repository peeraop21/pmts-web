using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.LogisticAndWarehouse
{
    public class TruckOptimizeMainModel
    {
        public List<TruckOptimizeViewModel> TruckOptimizeViewModels { get; set; }
        public TruckOptimize TruckOptimize { get; set; }
    }
}
