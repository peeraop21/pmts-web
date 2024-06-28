using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.BomRawMaterial;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class BomRawMaterialService : IBomRawMaterialService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IUnitMaterialAPIRepository _unitMaterialAPIRepository;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly IPPCRawMaterialMasterAPIRepository _ppcRawMaterialMasterAPIRepository;
        private readonly IPPCRawMaterialProductionBomAPIRepository _ppcRawMaterialProductionBomAPIRepository;
        private readonly IMoDataAPIRepository _moDataAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public BomRawMaterialService(IHttpContextAccessor httpContextAccessor,
            IUnitMaterialAPIRepository unitMaterialAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository,
            IPPCRawMaterialMasterAPIRepository ppcRawMaterialMasterAPIRepository,
            IPPCRawMaterialProductionBomAPIRepository ppcRawMaterialProductionBomAPIRepository,
            IMoDataAPIRepository moDataAPIRepository,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitMaterialAPIRepository = unitMaterialAPIRepository;
            _masterDataAPIRepository = masterDataAPIRepository;
            _ppcRawMaterialMasterAPIRepository = ppcRawMaterialMasterAPIRepository;
            _ppcRawMaterialProductionBomAPIRepository = ppcRawMaterialProductionBomAPIRepository;
            _moDataAPIRepository = moDataAPIRepository;


            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }

            this.mapper = mapper;
        }

        public List<SelectListItem> GetListUnitOfMeasureCode()
        {
            var tmp = JsonConvert.DeserializeObject<List<UnitMaterial>>(_unitMaterialAPIRepository.GetUnitMaterialList(_factoryCode, _token));
            List<SelectListItem> lst = new List<SelectListItem>();
            var tmpselect = tmp.Select(x => new { x.Id, x.Name }).Distinct();
            foreach (var item in tmpselect)
            {
                SelectListItem op = new SelectListItem();
                op.Value = item.Id.ToString();
                op.Text = item.Name;
                lst.Add(op);
            }
            return lst;
        }
        public List<MasterData> SearchMasterDataByMaterialNo(string materialNo)
        {
            var masterDataList = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.SearchMasterDataByMaterialNo(_factoryCode, materialNo, _token));
            return masterDataList;
        }
        public List<PpcRawMaterialMaster> SearchPPCRawMaterialMasterByMaterialNo(string materialNo, string materialDesc)
        {
            var ppcRawMaterialMasterLiat = JsonConvert.DeserializeObject<List<PpcRawMaterialMaster>>(_ppcRawMaterialMasterAPIRepository.SearchPPCRawMaterialMasterByMaterialNo(_factoryCode, materialNo, materialDesc, _token));
            return ppcRawMaterialMasterLiat;
        }

        public List<RawMaterialLineFront> GetRawMaterialProductionBomByFGMaterial(string fgMaterial)
        {
            var rawMaterialProductionBomList = JsonConvert.DeserializeObject<List<PpcRawMaterialProductionBom>>(_ppcRawMaterialProductionBomAPIRepository.GetPPCRawMaterialProductionBOMsByFgMaterial(_factoryCode, fgMaterial, _token));
            var lstMesureCode = this.GetListUnitOfMeasureCode();
            var model = new List<RawMaterialLineFront>();
            model = mapper.Map<List<RawMaterialLineFront>>(rawMaterialProductionBomList);
            if (model.Count > 0)
            {
                model.ForEach(a => a.SizeUom = !string.IsNullOrEmpty(a.SizeUom) ? lstMesureCode.FirstOrDefault(p => p.Text.ToUpper().Equals(a.SizeUom.ToUpper()))?.Text : a.SizeUom);
            }
            return model;
        }
        public string CheckMaterialNo(string materialNo)
        {
            string result = JsonConvert.DeserializeObject<string>(_moDataAPIRepository.CheckMaterialNo(_factoryCode, materialNo, _token));
            return result;
        }
        public void SaveRawMaterialProductionBom(RawMaterialLineRequest model)
        {
            var newModel = new PpcRawMaterialProductionBom();

            foreach (var item in model.BomRawData)
            {
                newModel = JsonConvert.DeserializeObject<PpcRawMaterialProductionBom>(_ppcRawMaterialProductionBomAPIRepository.GetPPCRawMaterialProductionBOMByFgMaterialAndMaterialNo(_factoryCode, model.FgMaterial, item.MaterialNumber, _token));
                if (newModel == null)
                {
                    newModel = new PpcRawMaterialProductionBom();
                    newModel.CreateDate = System.DateTime.Now;
                    newModel.CreateBy = _username;
                }

                newModel.FgMaterial = model.FgMaterial;
                newModel.MaterialNumber = item.MaterialNumber;
                newModel.MaterialType = item.MaterialType;
                newModel.Plant = item.Plant;
                newModel.MaterialDescription = item.MaterialDescription;
                newModel.NetWeight = item.NetWeight;
                newModel.MaterialGroup = item.MaterialGroup;
                newModel.SizeUom = item.SizeUom;
                newModel.Width = item.Width;
                newModel.Length = item.Length;
                newModel.Lay = item.Lay;
                newModel.CutSize = item.CutSize;
                newModel.BomAmount = item.BomAmount;
                newModel.Uom = item.Uom;
                newModel.OldMaterialNumber = item.OldMaterialNumber;
                newModel.UpdateDate = System.DateTime.Now;
                newModel.UpdateBy = _username;

                if (newModel.Id != 0)
                {
                    _ppcRawMaterialProductionBomAPIRepository.UpdatePPCRawMaterialProductionBom(_factoryCode, JsonConvert.SerializeObject(newModel), _token);
                }
                else
                {
                    newModel.Plant = _factoryCode;
                    _ppcRawMaterialProductionBomAPIRepository.SaveRawMaterialProductionBom(_factoryCode, JsonConvert.SerializeObject(newModel), _token);
                }
            }
        }
        public void DeleteRawMaterial(int id)
        {
            PpcRawMaterialProductionBom model = new PpcRawMaterialProductionBom();
            model.Id = id;
            _ppcRawMaterialProductionBomAPIRepository.DeleteRawMaterial(_factoryCode, JsonConvert.SerializeObject(model), _token);
        }

        public List<PpcRawMaterialMaster> SearchPPCRawMaterialMastersByFactoryAndMaterialNoAndDescription(string materialNo, string materialDesc)
        {
            var ppcRawMaterialMasterLiat = JsonConvert.DeserializeObject<List<PpcRawMaterialMaster>>(_ppcRawMaterialMasterAPIRepository.GetPPCRawMaterialMasterByFactoryAndMaterialNoAndDescription(_factoryCode, materialNo, materialDesc, _token));
            return ppcRawMaterialMasterLiat;
        }

        public List<PpcRawMaterialMaster> SearchPPCRawMaterialMastersByFactoryCode()
        {
            var ppcRawMaterialMasterLiat = JsonConvert.DeserializeObject<List<PpcRawMaterialMaster>>(_ppcRawMaterialMasterAPIRepository.GetPPCRawMaterialMastersByFactoryCode(_factoryCode, _token));
            return ppcRawMaterialMasterLiat;
        }

        public void SaveRawMaterialMaster(PpcRawMaterialMaster model)
        {
            var data = JsonConvert.DeserializeObject<List<PpcRawMaterialMaster>>(_ppcRawMaterialMasterAPIRepository.GetPPCRawMaterialMasterByFactoryAndMaterialNo(_factoryCode, model.MaterialNumber, _token));
            if (data.Any())
            {
                throw new Exception("Duplicate MaterialNumber!!!");
            }
            model.DateOfCreation = DateTime.Now;
            model.UpdateDate = DateTime.Now;
            model.UpdateBy = _username;
            model.Plant = _factoryCode;
            _ppcRawMaterialMasterAPIRepository.SavePPCRawMaterialMaster(JsonConvert.SerializeObject(model), _token);
        }

        public void UpdateRawMaterialMaster(PpcRawMaterialMaster model)
        {
            model.UpdateDate = DateTime.Now;
            model.UpdateBy = _username;
            _ppcRawMaterialMasterAPIRepository.UpdatePPCRawMaterialMaster(JsonConvert.SerializeObject(model), _token);
        }

        public void DeleteRawMaterialMaster(int id)
        {
            var model = new PpcRawMaterialMaster();
            model = JsonConvert.DeserializeObject<PpcRawMaterialMaster>(_ppcRawMaterialMasterAPIRepository.GetPPCRawMaterialMasterById(_factoryCode, id, _token));
            if (model != null)
            {
                _ppcRawMaterialMasterAPIRepository.DeletePPCRawMaterialMaster(_factoryCode, JsonConvert.SerializeObject(model), _token);
            }
            else
            {
                throw new System.Exception("Can't delete this material number.");
            }
        }
    }
}
