using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
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
    public class BomService : IBomService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly IBomStructAPIRepository _bomStructAPIRepository;
        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;


        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public BomService(
            IHttpContextAccessor httpContextAccessor,
            IMasterDataAPIRepository masterDataAPIRepository,
            IBomStructAPIRepository bomStructAPIRepository,
            ICompanyProfileAPIRepository companyProfileAPIRepository
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _masterDataAPIRepository = masterDataAPIRepository;
            _bomStructAPIRepository = bomStructAPIRepository;
            _companyProfileAPIRepository = companyProfileAPIRepository;

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

        public void GetMatParent(ref BOMViewModel bomViewModel, string txtSearch, string ddlSearch)
        {
            //BOMViewModel bomViewModel = new BOMViewModel();

            MasterData masterData = new MasterData();
            List<BomStruct> bomStructList = new List<BomStruct>();

            if (ddlSearch == "MaterialNo")
            {

                bomViewModel.lstMasterData = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.SearchBomStructsByMaterialNo(_factoryCode, txtSearch, _token));
                //model.lstBomStructs = BomRepository.GetBomStructsByMatOrPC(txtSearch, ddlSearch);
                //masterData = context.MasterData.Where(x => (x.MaterialNo.Contains(txtSearch) && (x.MaterialType == "84" || x.MaterialType == "14" || x.MaterialType == "24"))).FirstOrDefault();
            }
            else if (ddlSearch == "PC")
            {
                bomViewModel.lstMasterData = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.SearchBomStructsBytxtSearch(_factoryCode, txtSearch, _token));
                //masterData = context.MasterData.Where(x => (x.Pc.Contains(txtSearch) && (x.MaterialType == "84" || x.MaterialType == "14" || x.MaterialType == "24"))).FirstOrDefault();
            }

            //if (masterData != null)
            //{
            //    bomViewModel.lstBomStructs = JsonConvert.DeserializeObject<List<BomStruct>>(_bomStructAPIRepository.SearchBomStructByMaterialNo(_factoryCode, txtSearch));

            //    //ret = context.BomStruct.Where(x => x.MaterialNo == masterData.MaterialNo && x.PdisStatus != "X").ToList();
            //}
            //else
            //{
            //    throw new Exception("Not Found");
            //}

        }

        public string GetAllGroupCompany()
        {
            var tmp = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            var data = tmp.Where(x => x.Plant == _factoryCode).Select(x => x.Group).FirstOrDefault();
            return data.ToString();
        }

        public List<Plants> GetAllPlant()
        {
            List<Plants> model = new List<Plants>();
            var tmp = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            foreach (var item in tmp.Where(x => x.StatusPmts == true).ToList())
            {
                Plants tmpplant = new Plants();
                tmpplant.Plant = item.Plant;
                tmpplant.Desc = item.Plant + "-" + item.ShortName;
                model.Add(tmpplant);
            }
            return model;
        }

        public void GetParentChildByMat(ref BOMViewModel bomViewModel, string Mat)
        {
            if (Mat != null)
            {
                bomViewModel.lstBomStructs = JsonConvert.DeserializeObject<List<BomStruct>>(_bomStructAPIRepository.SearchBomStructByMaterialNo(_factoryCode, Mat, _token));

                //ret = context.BomStruct.Where(x => x.MaterialNo == masterData.MaterialNo && x.PdisStatus != "X").ToList();
            }
            else
            {
                throw new Exception("Not Found");
            }

        }

        public BOMViewModel GetMatChild(BOMViewModel model)
        {

            //model.lstMasterData = new List<MasterData>();
            model.lstMasterData = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.GetMasterDataByBomChild(_factoryCode, model.MaterialNo, model.AbbservName, model.ProductCode, _token));


            return model;
        }

        public BOMViewModel GetBomStruct(string materialNo)
        {
            BOMViewModel model = new BOMViewModel();
            model.lstBomStructs = JsonConvert.DeserializeObject<List<BomStruct>>(_bomStructAPIRepository.SearchBomStructByMaterialNo(_factoryCode, materialNo, _token));
            //model.lstBomStructs = BomRepository.GetBomStructsByMat(context, materialNo);
            return model;
        }
        public void SaveBomStruct(BOMViewModel model)
        {
            //model.lstMasterData = new List<MasterData>();
            //model.lstMasterData = JsonConvert.DeserializeObject<List<BomStruct>>(_bomStructAPIRepository.SearchBomStructsByMat(_factoryCode, model.MaterialNo));
            //model.lstMasterData = _bomStructAPIRepository.GetMasterDataListChild(context, model);

            var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, model.MaterialNo, _token));

            var bomStruct = new BomStruct
            {
                MaterialNo = model.ParentMaterialNo,
                FactoryCode = _factoryCode,
                Plant = masterData.Plant,
                BomUsage = "5",
                WeighBom = Convert.ToDouble(model.Weight),
                PreviousBom = null, //
                Follower = model.MaterialNo,
                Amount = Convert.ToByte(model.Pieces),
                Unit = "PC",
                PdisStatus = "C",
                TranStatus = false,
                SapStatus = false

            };

            ParentModel bomStructParent = new ParentModel();
            bomStructParent.AppName = Globals.AppNameEncrypt;
            bomStructParent.SaleOrg = _saleOrg;
            bomStructParent.PlantCode = _factoryCode;
            bomStructParent.BomStruct = bomStruct;

            _masterDataAPIRepository.UpdateMasterDataTransStatusByBomStruct(_factoryCode, bomStruct.MaterialNo, "0", _token);
            _bomStructAPIRepository.SaveBomStruct(_factoryCode, JsonConvert.SerializeObject(bomStructParent), _token);
        }

        public string DeleteBomStruct(int Id)
        {
            var BomData = JsonConvert.DeserializeObject<BomStruct>(_bomStructAPIRepository.GetBomStructById(_factoryCode, Id, _token));

            BomData.PdisStatus = "X";

            ParentModel bomStructParent = new ParentModel();
            bomStructParent.AppName = Globals.AppNameEncrypt;
            bomStructParent.SaleOrg = _saleOrg;
            bomStructParent.PlantCode = _factoryCode;
            bomStructParent.BomStruct = BomData;
            string jsonString = JsonConvert.SerializeObject(bomStructParent);

            _masterDataAPIRepository.UpdateMasterDataTransStatusByBomStruct(_factoryCode, BomData.MaterialNo, "0", _token);
            _bomStructAPIRepository.UpdateBomStruct(_factoryCode, jsonString, _token);

            return BomData.MaterialNo;
        }


        public void GetMatParentToProductCatalog(ref BOMViewModel bomViewModel, string txtSearch, string ddlSearch, string FactoryCode)
        {
            //BOMViewModel bomViewModel = new BOMViewModel();

            MasterData masterData = new MasterData();
            List<BomStruct> bomStructList = new List<BomStruct>();

            if (ddlSearch == "MaterialNo")
            {

                bomViewModel.lstMasterData = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.SearchBomStructsByMaterialNo(FactoryCode, txtSearch, _token));
                //model.lstBomStructs = BomRepository.GetBomStructsByMatOrPC(txtSearch, ddlSearch);
                //masterData = context.MasterData.Where(x => (x.MaterialNo.Contains(txtSearch) && (x.MaterialType == "84" || x.MaterialType == "14" || x.MaterialType == "24"))).FirstOrDefault();
            }
            else if (ddlSearch == "PC")
            {
                bomViewModel.lstMasterData = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.SearchBomStructsBytxtSearch(FactoryCode, txtSearch, _token));
                //masterData = context.MasterData.Where(x => (x.Pc.Contains(txtSearch) && (x.MaterialType == "84" || x.MaterialType == "14" || x.MaterialType == "24"))).FirstOrDefault();
            }

            bomViewModel.lstMasterData = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.GetMasterDataByBomChild(_factoryCode, bomViewModel.MaterialNo, bomViewModel.AbbservName, bomViewModel.ProductCode, _token));

            //if (masterData != null)
            //{
            //    bomViewModel.lstBomStructs = JsonConvert.DeserializeObject<List<BomStruct>>(_bomStructAPIRepository.SearchBomStructByMaterialNo(_factoryCode, txtSearch));

            //    //ret = context.BomStruct.Where(x => x.MaterialNo == masterData.MaterialNo && x.PdisStatus != "X").ToList();
            //}
            //else
            //{
            //    throw new Exception("Not Found");
            //}

        }

        public void EditBOMStruct(BomStruct bomStruct)
        {
            ParentModel bomStructParent = new ParentModel();
            bomStructParent.AppName = Globals.AppNameEncrypt;
            bomStructParent.SaleOrg = _saleOrg;
            bomStructParent.PlantCode = _factoryCode;

            var datas = JsonConvert.DeserializeObject<BomStruct>(_bomStructAPIRepository.GetBomStructById(_factoryCode, bomStruct.Id, _token));
            bomStructParent.BomStruct = bomStruct;
            bomStructParent.BomStruct.CreatedBy = datas.CreatedBy;
            bomStructParent.BomStruct.CreatedDate = datas.CreatedDate;
            bomStructParent.BomStruct.UpdatedBy = _username;
            bomStructParent.BomStruct.UpdatedDate = DateTime.Now;

            _masterDataAPIRepository.UpdateMasterDataTransStatusByBomStruct(_factoryCode, bomStruct.MaterialNo, "0", _token);
            _bomStructAPIRepository.UpdateBomStruct(_factoryCode, JsonConvert.SerializeObject(bomStructParent), _token);
        }

        public void CopyMatNewPlant(string parentmat, string plants)
        {
            _bomStructAPIRepository.CopyBomStrucToNewPlant(parentmat, plants, _factoryCode, _token);
        }

        public MasterData GetMasterdataByMaterial(string parentmat, string plants)
        {
            var data = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(plants, parentmat, _token));
            return data;
        }

        public void ResentBOM(string materialNo)
        {
            var bomStructs = JsonConvert.DeserializeObject<List<BomStruct>>(_bomStructAPIRepository.SearchBomStructByMaterialNo(_factoryCode, materialNo, _token));
            foreach (var bomStruct in bomStructs)
            {
                var parentModel = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    BomStruct = bomStruct
                };
                parentModel.BomStruct = bomStruct;
                parentModel.BomStruct.UpdatedBy = _username;
                parentModel.BomStruct.UpdatedDate = DateTime.Now;
                parentModel.BomStruct.PdisStatus = bomStruct.SapStatus ? "M" : "C";
                parentModel.BomStruct.TranStatus = false;
                parentModel.BomStruct.SapStatus = false;

                //_masterDataAPIRepository.UpdateMasterDataTransStatusByBomStruct(_factoryCode, bomStruct.MaterialNo, "0", _token);
                _bomStructAPIRepository.UpdateBomStruct(_factoryCode, JsonConvert.SerializeObject(parentModel), _token);
            }
        }
    }
}
