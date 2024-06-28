
using PMTs.DataAccess.ModelView.MaintenanceScoreGap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceScoreGapService
    {
        void GetScoreGap(MaintenanceScoreGapViewModel maintenanceScoreGapViewModel);
        void SaveScoreGap(MaintenanceScoreGapViewModel maintenanceScoreGapViewModel);
        void UpdateScoreGap(ScoreGapViewModel ScoreGapViewModel);
    }
}
