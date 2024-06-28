using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceHoneyPaperService
    {
        List<HoneyPaper> GetAllHoneypaper();

        HoneyPaper GetHoneypaperByGrade(string grade);

        void CreateHoneypaper(HoneyPaper honeyPaper);

        void UpdateHoneypaper(HoneyPaper honeyPaper);
    }
}
