using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenanceHoneyPaperService : IMaintenanceHoneyPaperService
    {
        private readonly IHoneyPaperAPIRepository honeyPaperAPIRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceHoneyPaperService(
            IHoneyPaperAPIRepository honeyPaperAPIRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            this.honeyPaperAPIRepository = honeyPaperAPIRepository;
            this.httpContextAccessor = httpContextAccessor;

            // Initialize SaleOrg and PlantCode from Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }

        }

        public void CreateHoneypaper(HoneyPaper honeyPaper)
        {
            HoneyPaper existHoneyPaper = new HoneyPaper();
            existHoneyPaper = JsonConvert.DeserializeObject<HoneyPaper>(honeyPaperAPIRepository.GetHoneyPaperByGrade(_factoryCode, honeyPaper.Grade, _token));

            if (existHoneyPaper == null)
            {
                honeyPaper.CreatedBy = _username;
                honeyPaper.CreatedDate = DateTime.Now;
                honeyPaperAPIRepository.CreateHoneyPaper(_factoryCode, JsonConvert.SerializeObject(honeyPaper), _token);
            }
        }

        public List<HoneyPaper> GetAllHoneypaper()
        {
            var honeyPapers = JsonConvert.DeserializeObject<List<HoneyPaper>>(honeyPaperAPIRepository.GetAllHoneyPaper(_factoryCode, _token));

            return honeyPapers;
        }

        public HoneyPaper GetHoneypaperByGrade(string grade)
        {
            var honeyPaper = JsonConvert.DeserializeObject<HoneyPaper>(honeyPaperAPIRepository.GetHoneyPaperByGrade(_factoryCode, grade, _token));

            return honeyPaper;
        }

        public void UpdateHoneypaper(HoneyPaper honeyPaper)
        {
            HoneyPaper tempHoneyPaper = new HoneyPaper();
            tempHoneyPaper = JsonConvert.DeserializeObject<HoneyPaper>(honeyPaperAPIRepository.GetHoneyPaperByGrade(_factoryCode, honeyPaper.Grade, _token));

            if (tempHoneyPaper != null)
            {
                honeyPaper.UpdatedBy = _username;
                honeyPaper.UpdatedDate = DateTime.Now;
                honeyPaperAPIRepository.UpdateHoneyPaper(_factoryCode, JsonConvert.SerializeObject(honeyPaper), _token);
            }
        }
    }
}
