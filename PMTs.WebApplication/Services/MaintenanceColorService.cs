using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceColor;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenanceColorService : IMaintenanceColorService
    {
        IHttpContextAccessor _httpContextAccessor;

        private readonly IColorAPIRepository _ColorAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceColorService(IHttpContextAccessor httpContextAccessor, IColorAPIRepository ColorAPIRepository)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _ColorAPIRepository = ColorAPIRepository;
            // Initialize SaleOrg and PlantCode from Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        public void GetColor(MaintenanceColorViewModel maintenanceColorViewModel)
        {
            // Convert Json String to List Object
            var ColorList = JsonConvert.DeserializeObject<List<Color>>(_ColorAPIRepository.GetColorMaintainList(_factoryCode, _token));

            //var ColorModelViewList = Mapper.Map<List<Color>, List<ColorViewModel>>(ColorList);

            maintenanceColorViewModel.ColorViewModelList = new List<Color>();
            maintenanceColorViewModel.ColorViewModelList = ColorList;

        }

        public void SaveColor(MaintenanceColorViewModel model)
        {
            ParentModel ColorModel = new ParentModel();
            ColorModel.AppName = Globals.AppNameEncrypt;
            ColorModel.FactoryCode = _factoryCode;
            ColorModel.PlantCode = _factoryCode;

            //model.ColorViewModel.ColorStatus = true;
            model.ColorViewModel.FactoryCode = _factoryCode;

            //ColorModel.Color = Mapper.Map<ColorViewModel, Color>(model.ColorViewModel);
            ColorModel.Color = model.ColorViewModel;
            ColorModel.Color.CreatedDate = DateTime.Now;
            ColorModel.Color.CreatedBy = _username;
            ColorModel.Color.Id = 0;

            string ColorListJsonString = JsonConvert.SerializeObject(ColorModel);

            //Check Duplicate shade in colors
            if (IsExistShade(ColorModel.Color.Shade, ColorModel.Color.Id))
            {
                throw new Exception($"Your factory has been created shade: {ColorModel.Color.Shade}!");
            }

            _ColorAPIRepository.SaveColor(_factoryCode, ColorListJsonString, _token);

        }
        public void UpdateColor(Color ColorViewModel)
        {
            ParentModel ColorModel = new ParentModel();
            ColorModel.AppName = Globals.AppNameEncrypt;
            ColorModel.FactoryCode = _factoryCode;
            ColorModel.PlantCode = _factoryCode;
            ColorViewModel.FactoryCode = _factoryCode;

            ColorModel.Color = ColorViewModel;
            ColorModel.Color.UpdatedDate = DateTime.Now;
            ColorModel.Color.UpdatedBy = _username;

            string jsonString = JsonConvert.SerializeObject(ColorModel);

            //Check Duplicate shade in colors
            if (IsExistShade(ColorModel.Color.Shade, ColorModel.Color.Id))
            {
                throw new Exception($"Your factory has been created shade: {ColorModel.Color.Shade}!");
            }
            _ColorAPIRepository.UpdateColor(_factoryCode, jsonString, _token);
        }
        public bool IsExistShade(string shade, int? id)
        {
            var isexist = false;
            var color = JsonConvert.DeserializeObject<Color>(_ColorAPIRepository.GetColorByShadeAndFactoryCode(shade, _factoryCode, _token));
            if (color != null && id != color.Id)
            {
                isexist = true;
            }
            return isexist;
        }
    }
}
