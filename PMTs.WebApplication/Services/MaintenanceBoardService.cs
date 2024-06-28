using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceBoard;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenanceBoardService : IMaintenanceBoardService
    {
        IHttpContextAccessor _httpContextAccessor;

        private readonly IFluteAPIRepository _fluteAPIRepository;
        private readonly IBoardCombineAPIRepository _boardCombineAPIRepository;
        private readonly IBoardCombineAccAPIRepository _boardCombineAccAPIRepository;
        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;
        private readonly IProductTypeAPIRepository _productTypeAPIRepository;
        private readonly IHierarchyLv2APIRepository _hierarchyLv2APIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private string _token;


        public MaintenanceBoardService(IHttpContextAccessor httpContextAccessor, IFluteAPIRepository fluteAPIRepository, IBoardCombineAPIRepository boardCombineAPIRepository, IBoardCombineAccAPIRepository boardCombineAccAPIRepository, ICompanyProfileAPIRepository companyProfileAPIRepository, IProductTypeAPIRepository productTypeAPIRepository, IHierarchyLv2APIRepository hierarchyLv2APIRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _fluteAPIRepository = fluteAPIRepository;
            _boardCombineAPIRepository = boardCombineAPIRepository;
            _companyProfileAPIRepository = companyProfileAPIRepository;
            _productTypeAPIRepository = productTypeAPIRepository;
            _hierarchyLv2APIRepository = hierarchyLv2APIRepository;

            var userSession = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSession != null)
            {
                _username = userSession.UserName;
                _saleOrg = userSession.SaleOrg;
                _factoryCode = userSession.FactoryCode;
                _token = userSession.Token;

            }

            _boardCombineAccAPIRepository = boardCombineAccAPIRepository;
        }

        //public MaintenanceBoardModel GetBoard(MaintenanceBoardModel maintenanceBoardModel)
        //{
        //    maintenanceBoardModel.ModelToSave = new BoardModel();
        //    maintenanceBoardModel.ModelToSave.FluteSelectList = JsonConvert.DeserializeObject<List<Flute>>(_fluteAPIRepository.GetFluteList(_factoryCode));

        //    // Convert Json String to List Object
        //    var BoardList = JsonConvert.DeserializeObject<List<BoardModel>>(_boardCombineAPIRepository.GetBoardCombineList(_factoryCode));

        //    maintenanceBoardModel.BoardModelList = BoardList;
        //    return maintenanceBoardModel ;
        //}

        //public MaintenanceBoardModel GetSelectFlute(MaintenanceBoardModel maintenanceBoardModel)
        //{
        //    maintenanceBoardModel.ModelToSave = new BoardModel();
        //    maintenanceBoardModel.ModelToSave.FluteSelectList = JsonConvert.DeserializeObject<List<Flute>>(_fluteAPIRepository.GetFluteList(_factoryCode));
        //    return maintenanceBoardModel;
        //}



        //==================================================

        public BoardCombindMainTainModel getFristBoardCombind()
        {
            var model = JsonConvert.DeserializeObject<BoardCombindMainTainModel>(_boardCombineAPIRepository.GetAllDataMainTain(_factoryCode, _token));
            var tmp = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            var data = tmp.Where(x => x.Plant == _factoryCode).Select(x => x.Group).FirstOrDefault();
            model.GroupCompany = data.ToString();
            return model;
        }

        public BoardCombindMainTainModel getMaxcodeBoardCombind()
        {
            var model = JsonConvert.DeserializeObject<BoardCombindMainTainModel>(_boardCombineAPIRepository.GetMaxcode(_factoryCode, _token));
            return model;
        }

        public string GenerateBoardCombindCode()
        {
            return JsonConvert.DeserializeObject<string>(_boardCombineAPIRepository.GenerateCode(_factoryCode, _token));
        }
        public string GenerateFileDataForSAP(ExportDataForSAPRequest request)
        {
            var data = JsonConvert.DeserializeObject<ExportDataForSAPResponse>(_boardCombineAPIRepository.GenerateDataForSAP(_factoryCode, JsonConvert.SerializeObject(request), _token));

            string textData = string.Empty;

            var dataSorted = data.Items.OrderBy(x => x.Code).ToList();

            for (int i = 0; i < dataSorted.Count(); i++)
            {
                textData += dataSorted[i].Code + "    " + dataSorted[i].Board + "\n";
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(textData);

            return Convert.ToBase64String(byteArray);
        }



        public BoardCombindMainTainModel getAllBoardspectByCode(string code)
        {
            var model = JsonConvert.DeserializeObject<BoardCombindMainTainModel>(_boardCombineAPIRepository.GetAllBoardspectByCode(_factoryCode, code, _token));
            return model;
        }

        public bool AddBoardcombind(BoardCombindMainTainModel model)
        {
            var result = JsonConvert.DeserializeObject<bool>(_boardCombineAPIRepository.AddBoardcombind(_factoryCode, JsonConvert.SerializeObject(model), _token));
            return result;
        }
        public void UpdateBoardcombind(BoardCombindMainTainModel model)
        {
            _boardCombineAPIRepository.UpdateBoardCombind(_factoryCode, JsonConvert.SerializeObject(model), _token);
        }


        public BoardCombindMainTainModel getBoardCombindAccCode(string code)
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            model.editCostFieldsModel = JsonConvert.DeserializeObject<EditCostFieldsModel>(_boardCombineAccAPIRepository.GetBoardCombineAcc(_factoryCode, code, _token));
            return model;
        }

        public void ManageBoardcombindAcc(List<BoardCombindAccUpdate> model)
        {
            model.ForEach(c => c.FactoryCode = _factoryCode);
            model.ForEach(c => c.UpdateBy = _username);
            _boardCombineAccAPIRepository.UpdateBoardCombineAcc(_factoryCode, JsonConvert.SerializeObject(model), _token);
        }


    }
}
