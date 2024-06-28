using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenancePlantCostField;
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
    public class MaintenancePlantCostFieldService : IMaintenancePlantCostFieldService
    {
        public readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IPlantCostFieldAPIRepository _plantCostFieldAPIRepository;
        public readonly IMapCostAPIRepository _mapCostAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenancePlantCostFieldService(IHttpContextAccessor httpContextAccessor,
            IPlantCostFieldAPIRepository plantCostFieldAPIRepository,
            IMapCostAPIRepository mapCostAPIRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _plantCostFieldAPIRepository = plantCostFieldAPIRepository;
            _mapCostAPIRepository = mapCostAPIRepository;

            // Initialize User Data From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        public void GetPlantCostField(ref List<PlantCostFieldViewModel> plantCostFields)
        {
            var mapCosts = JsonConvert.DeserializeObject<List<MapCost>>(_mapCostAPIRepository.GetMapCostList(_factoryCode, _token)).Where(m => m.Active.HasValue && m.Active.Value == true).Select(m => m.CostField).Distinct().ToList();

            var plantCostFieldList = JsonConvert.DeserializeObject<List<PlantCostField>>(_plantCostFieldAPIRepository.GetPlantCostFields(_factoryCode, _token)).Where(p => p.FactoryCode == _factoryCode).ToList();

            var num = 0;
            var numofSS = mapCosts.Count;
            foreach (var mapCost in mapCosts)
            {
                var plantCostFieldViewModel = new PlantCostFieldViewModel();
                var existPlantCostField = plantCostFieldList.FirstOrDefault(p => p.CostField.ToLower().Trim() == mapCost.ToLower().Trim());
                if (existPlantCostField != null)
                {
                    //plantCostFieldViewModel.Id = existPlantCostField.Id.ToString();
                    plantCostFieldViewModel.Id = (numofSS + existPlantCostField.Id).ToString();
                    plantCostFieldViewModel.CostField = existPlantCostField.CostField;
                    plantCostFieldViewModel.FactoryCode = existPlantCostField.FactoryCode;
                    plantCostFieldViewModel.SelectStatus = true;
                    plantCostFieldViewModel.CostField = existPlantCostField.CostField;
                }
                else
                {
                    //plantCostFieldViewModel.Id = CheckDupicateId(num, plantCostFieldList).ToString();
                    plantCostFieldViewModel.Id = num.ToString();
                    plantCostFieldViewModel.CostField = mapCost;
                    plantCostFieldViewModel.SelectStatus = false;

                    num++;
                }
                plantCostFields.Add(plantCostFieldViewModel);

            }
        }

        public void UpdatePlantCostField(string plantCostFieldArr)
        {
            var plantCostFieldSelects = JsonConvert.DeserializeObject<List<PlantCostFieldViewModel>>(plantCostFieldArr);
            var plantCostFieldsModel = new List<PlantCostField>();

            foreach (var plantCostFieldSelect in plantCostFieldSelects)
            {
                if (plantCostFieldSelect.SelectStatus)
                {
                    //save new plant cost field

                    var plantCostField = new PlantCostField
                    {
                        CostField = plantCostFieldSelect.CostField,
                        FactoryCode = _factoryCode
                    };

                    plantCostFieldsModel.Add(plantCostField);
                }
            }

            //Delete and add new PlantCostFields
            _plantCostFieldAPIRepository.CreatePlantCostFields(_factoryCode, JsonConvert.SerializeObject(plantCostFieldsModel), _token);
        }

        private int CheckDupicateId(int num, List<PlantCostField> plantCostFields)
        {
            var exitId = plantCostFields.FirstOrDefault(p => p.Id == num);
            if (exitId != null)
            {
                num++;
                CheckDupicateId(num, plantCostFields);
            }
            else
            {
                num++;
            }

            return num;
        }
    }
}
