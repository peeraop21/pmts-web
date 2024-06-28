using Microsoft.AspNetCore.Http;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IReCalculateTrimService
    {
        List<Flute> GetFlutesByFactoryCode();
        ReCalculateTrimViewModel GetFlutesAndMachinesByFactoryCode();
        void ReCalculateTrim(string flute, int numberOfProgress, int processLimit, ref ChangeReCalculateTrimModel model, ref List<ReCalculateTrimModel> reCalculateTrims, ref List<Routing> routings);
        ChangeReCalculateTrimModel GetReCalculateTrim(string flute, string machine, string boxType, string printMethod, string proType);
        void SaveReCalculateTrim(ChangeReCalculateTrimModel model);
        void ImportReCalculateTrimFromFile(IFormFile file, ref string exceptionMessage);
    }
}
