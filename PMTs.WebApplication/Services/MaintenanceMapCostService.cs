using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceMapCost;
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
    public class MaintenanceMapCostService : IMaintenanceMapCostService
    {
        public readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IMapCostAPIRepository _mapCostAPIRepository;
        public readonly IBoardCombineAccAPIRepository _boardCombineAccAPIRepository;
        public readonly IPlantCostFieldAPIRepository _plantCostFieldAPIRepository;
        public readonly IProductTypeAPIRepository _productTypeAPIRepository;
        public readonly IHierarchyLv2APIRepository hierarchyLv2APIRepository;
        public readonly IHierarchyLv3APIRepository hierarchyLv3APIRepository;
        public readonly IHierarchyLv4APIRepository hierarchyLv4APIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceMapCostService(IMapCostAPIRepository mapCostAPIRepository,
            IHttpContextAccessor httpContextAccessor,
            IBoardCombineAccAPIRepository boardCombineAccAPIRepository,
            IPlantCostFieldAPIRepository plantCostFieldAPIRepository,
            IProductTypeAPIRepository productTypeAPIRepository,
            IHierarchyLv2APIRepository hierarchyLv2APIRepository,
            IHierarchyLv3APIRepository hierarchyLv3APIRepository,
            IHierarchyLv4APIRepository hierarchyLv4APIRepository
            )
        {
            this._mapCostAPIRepository = mapCostAPIRepository;
            this._httpContextAccessor = httpContextAccessor;
            this._boardCombineAccAPIRepository = boardCombineAccAPIRepository;
            this._plantCostFieldAPIRepository = plantCostFieldAPIRepository;
            this._productTypeAPIRepository = productTypeAPIRepository;
            this.hierarchyLv2APIRepository = hierarchyLv2APIRepository;
            this.hierarchyLv3APIRepository = hierarchyLv3APIRepository;
            this.hierarchyLv4APIRepository = hierarchyLv4APIRepository;

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

        #region MapCost 
        public void CreateMapCost(MaintenanceMapCostViewModel maintenanceMapCostViewModel)
        {
            var mapCost = new MapCost
            {
                CostField = maintenanceMapCostViewModel.CostField,
                Description = maintenanceMapCostViewModel.Description,
                Hierarchy2 = maintenanceMapCostViewModel.Hierarchy2,
                Hierarchy3 = maintenanceMapCostViewModel.Hierarchy3,
                Hierarchy4 = maintenanceMapCostViewModel.Hierarchy4,
                CreatedBy = _username,
                CreatedDate = DateTime.Now
            };

            _mapCostAPIRepository.CreateMapCost(JsonConvert.SerializeObject(mapCost), _token);

            //add column in board combine acc
            var mapCostSession = SessionExtentions.GetSession<MaintenanceMapCostViewModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceMapCostViewModel");

            if (mapCostSession.MapCosts.FirstOrDefault(m => m.CostField.Equals(mapCost.CostField) && m.Hierarchy2.Equals(mapCost.Hierarchy2) && m.Hierarchy3.Equals(mapCost.Hierarchy3) && m.Hierarchy4.Equals(mapCost.Hierarchy4)) == null)
            {
                _boardCombineAccAPIRepository.AddBoardCombineAccColumn(_factoryCode, maintenanceMapCostViewModel.CostField, _token);
                //_boardCombineAccAPIRepository.AddColumn(maintenanceMapCostViewModel.CostField);
            }
            else
            {
                throw new Exception("Can't add new cost field!");
            }
        }

        public void UpdateMapCost(MaintenanceMapCostViewModel maintenanceMapCostViewModel)
        {
            var mapCostSession = SessionExtentions.GetSession<MaintenanceMapCostViewModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceMapCostViewModel");
            var mapCostUpdate = mapCostSession.MapCosts.FirstOrDefault(m => m.Id == Convert.ToInt32(maintenanceMapCostViewModel.Id));

            var mapCost = new MapCost
            {
                Id = Convert.ToInt32(maintenanceMapCostViewModel.Id),
                CostField = maintenanceMapCostViewModel.CostField,
                Description = maintenanceMapCostViewModel.Description,
                Hierarchy2 = maintenanceMapCostViewModel.Hierarchy2,
                Hierarchy3 = maintenanceMapCostViewModel.Hierarchy3,
                Hierarchy4 = maintenanceMapCostViewModel.Hierarchy4
            };

            if (mapCostUpdate != null)
            {
                mapCost.CreatedDate = mapCostUpdate.CreatedDate;
                mapCost.CreatedBy = mapCostUpdate.CreatedBy;
                mapCost.UpdatedDate = DateTime.Now;
                mapCost.UpdatedBy = _username;
            }

            _mapCostAPIRepository.UpdateMapCost(JsonConvert.SerializeObject(mapCost), _token);

            //check for update board combine acc
            if (mapCostSession.MapCosts.FirstOrDefault(m => m.CostField == mapCost.CostField) == null)
            {
                //update cost field name in board combine acc
                /*_boardCombineAccAPIRepository.ChangeColumn(mapCostUpdate.CostField, mapCost.CostField);*/
            }
        }

        public void DeleteMapCost(string id)
        {
            var mapCostSession = SessionExtentions.GetSession<MaintenanceMapCostViewModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceMapCostViewModel");
            var mapCost = mapCostSession.MapCosts.FirstOrDefault(m => m.Id == Convert.ToInt32(id));

            var mapCosts = JsonConvert.DeserializeObject<List<MapCost>>(_mapCostAPIRepository.GetMapCostList(_factoryCode, _token));

            //check map cost in use in plant cost field
            //if (!_plantCostFieldAPIRepository.CheckCostFieldinUse(_factoryCode, mapCost.CostField))
            //{
            //    _mapCostAPIRepository.DeleteMapCost(JsonConvert.SerializeObject(mapCost));

            //    //delete column in board combine acc
            //    //_boardCombineAccAPIRepository.DropColumn(mapCost.CostField);
            //}
        }

        public void GetMapCost(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel)
        {
            var mapcosts = JsonConvert.DeserializeObject<List<MapCost>>(_mapCostAPIRepository.GetMapCostList(_factoryCode, _token));
            var productType = JsonConvert.DeserializeObject<List<ProductType>>(_productTypeAPIRepository.GetProductTypeList(_factoryCode, _token));
            //var Hierarchy2s = mapcosts.Select(m => m.Hierarchy2).Distinct().ToList();
            var Hierarchy2sSelect = productType.OrderBy(p => p.HierarchyLv2).Select(m => m.HierarchyLv2).Distinct().ToList();
            var Hierarchy3sSelect = mapcosts.Select(m => m.Hierarchy3).Distinct().ToList();
            var Hierarchy4sSelect = mapcosts.Select(m => m.Hierarchy4).Distinct().ToList();

            maintenanceMapCostViewModel.MapCosts = mapcosts;
            maintenanceMapCostViewModel.Hierarchy2SelectList = Hierarchy2sSelect.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() });
            maintenanceMapCostViewModel.Hierarchy3SelectList = Hierarchy3sSelect.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() });
            maintenanceMapCostViewModel.Hierarchy4SelectList = Hierarchy4sSelect.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() });

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceMapCostViewModel", maintenanceMapCostViewModel);
        }
        #endregion

        #region HierarchyLv3 
        public void GetHierarchyLv3(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel)
        {
            maintenanceMapCostViewModel.Hierarchy3s = JsonConvert.DeserializeObject<List<HierarchyLv3>>(hierarchyLv3APIRepository.GetAllHierarchyLv3s(_factoryCode, _token));
        }

        public void SaveHierarchyLv3(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel, HierarchyCreateModel hierarchyCreateModel)
        {
            var hierarchyLv3 = new HierarchyLv3();
            hierarchyLv3.Id = 0;
            hierarchyLv3.Hierarchy3 = hierarchyCreateModel.Name;
            hierarchyLv3.Description = hierarchyCreateModel.Description;
            hierarchyLv3.CreatedBy = _username;
            hierarchyLv3.CreatedDate = DateTime.Now;

            hierarchyLv3APIRepository.SaveHierarchy3(_factoryCode, JsonConvert.SerializeObject(hierarchyLv3), _token);

            maintenanceMapCostViewModel.Hierarchy3s = JsonConvert.DeserializeObject<List<HierarchyLv3>>(hierarchyLv3APIRepository.GetAllHierarchyLv3s(_factoryCode, _token));
        }

        public void UpdateHierarchyLv3(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel, HierarchyCreateModel hierarchyCreateModel)
        {
            var hierarchyLv3 = new HierarchyLv3();
            hierarchyLv3.Id = hierarchyCreateModel.Id;
            hierarchyLv3.Hierarchy3 = hierarchyCreateModel.Name;
            hierarchyLv3.Description = hierarchyCreateModel.Description;
            hierarchyLv3.CreatedBy = hierarchyCreateModel.CreatedBy;
            hierarchyLv3.CreatedDate = hierarchyCreateModel.CreatedDate;
            hierarchyLv3.UpdatedBy = _username;
            hierarchyLv3.UpdatedDate = DateTime.Now;

            hierarchyLv3APIRepository.UpdateHierarchy3(_factoryCode, JsonConvert.SerializeObject(hierarchyLv3), _token);

            maintenanceMapCostViewModel.Hierarchy3s = JsonConvert.DeserializeObject<List<HierarchyLv3>>(hierarchyLv3APIRepository.GetAllHierarchyLv3s(_factoryCode, _token));
        }

        #endregion

        #region HierarchyLv4 
        public void GetHierarchyLv4(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel)
        {
            maintenanceMapCostViewModel.Hierarchy4s = JsonConvert.DeserializeObject<List<HierarchyLv4>>(hierarchyLv4APIRepository.GetAllHierarchyLv4s(_factoryCode, _token));
        }

        public void SaveHierarchyLv4(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel, HierarchyCreateModel hierarchyCreateModel)
        {
            var hierarchyLv4 = new HierarchyLv4();
            hierarchyLv4.Id = 0;
            hierarchyLv4.Hierarchy4 = hierarchyCreateModel.Name;
            hierarchyLv4.Description = hierarchyCreateModel.Description;
            hierarchyLv4.CreatedBy = _username;
            hierarchyLv4.CreatedDate = DateTime.Now;

            hierarchyLv4APIRepository.SaveHierarchy4(_factoryCode, JsonConvert.SerializeObject(hierarchyLv4), _token);

            maintenanceMapCostViewModel.Hierarchy4s = JsonConvert.DeserializeObject<List<HierarchyLv4>>(hierarchyLv4APIRepository.GetAllHierarchyLv4s(_factoryCode, _token));
        }

        public void UpdateHierarchyLv4(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel, HierarchyCreateModel hierarchyCreateModel)
        {
            var hierarchyLv4 = new HierarchyLv4();
            hierarchyLv4.Id = hierarchyCreateModel.Id;
            hierarchyLv4.Hierarchy4 = hierarchyCreateModel.Name;
            hierarchyLv4.Description = hierarchyCreateModel.Description;
            hierarchyLv4.CreatedBy = hierarchyCreateModel.CreatedBy;
            hierarchyLv4.CreatedDate = hierarchyCreateModel.CreatedDate;
            hierarchyLv4.UpdatedBy = _username;
            hierarchyLv4.UpdatedDate = DateTime.Now;

            hierarchyLv4APIRepository.UpdateHierarchy4(_factoryCode, JsonConvert.SerializeObject(hierarchyLv4), _token);

            maintenanceMapCostViewModel.Hierarchy4s = JsonConvert.DeserializeObject<List<HierarchyLv4>>(hierarchyLv4APIRepository.GetAllHierarchyLv4s(_factoryCode, _token));
        }

        #endregion
    }
}
