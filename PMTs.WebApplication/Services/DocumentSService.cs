using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess.ComplexModel;
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
    public class DocumentSService : IDocumentSService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IMapper mapper;

        private readonly INewProductService _newProductService;
        private readonly IPresaleService _presaleService;

        private readonly IDocumentSRepository _documentSRepository;
        private readonly IMoSpecAPIRepository moSpecAPIRepository;
        private readonly ICompanyProfileAPIRepository companyProfileAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public DocumentSService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment hostingEnvironment,
            IDocumentSRepository documentSRepository,
            IMoSpecAPIRepository moSpecAPIRepository,
            ICompanyProfileAPIRepository companyProfileAPIRepository,
            IMapper mapper
            )
        {
            this.moSpecAPIRepository = moSpecAPIRepository;
            this.companyProfileAPIRepository = companyProfileAPIRepository;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;

            // Initialize API Repository
            _documentSRepository = documentSRepository;
            // Initialize UserData From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        public CreateDocumentSModel GetDocumentS(string snumber)
        {
            CreateDocumentSModel model = new CreateDocumentSModel();
            model.ldocumentS = JsonConvert.DeserializeObject<List<DocumentS>>(_documentSRepository.GetDocumentS(_factoryCode, snumber, _token));
            return model;

        }

        public CreateDocumentSModel CreateDocumentS()
        {
            CreateDocumentSModel model = new CreateDocumentSModel();
            model.documentS = JsonConvert.DeserializeObject<DocumentS>(_documentSRepository.CreateNewDocumentS(_factoryCode, _username, _token));
            return model;

        }

        public DocumentSData GetOrderData(string orderitem)
        {
            DocumentSData model = new DocumentSData();
            model = JsonConvert.DeserializeObject<DocumentSData>(_documentSRepository.GetDataMo(_factoryCode, orderitem, _token));
            return model;
        }

        public string GetShortNameFacOfOutsourceByOrderItem(string orderItem)
        {
            var existMoSpec = JsonConvert.DeserializeObject<MoSpec>(moSpecAPIRepository.GetMoSpecByOrderItem(_factoryCode, orderItem, _token));
            var OsFactory = string.Empty;
            if (existMoSpec != null)
            {
                if (existMoSpec.SaleOrg == _saleOrg && existMoSpec.FactoryCode == _factoryCode)
                {
                    return string.Empty;
                }
                else
                {
                    var companyOfOs = JsonConvert.DeserializeObject<CompanyProfile>(companyProfileAPIRepository.GetCompanyProfileByPlant(existMoSpec.FactoryCode, _token));
                    return companyOfOs.ShortName;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public List<DocumentSlist> GetDocumentSList(string Snumber)
        {
            //DocumentSData model = new DocumentSData();
            var modeltemp = JsonConvert.DeserializeObject<List<DocumentSlist>>(_documentSRepository.GetDocumentSList(_factoryCode, Snumber, _token));
            return modeltemp;
        }

        public void AddDocList(ManageDocument model)
        {
            model.Username = _username;
            _documentSRepository.SaveDocumentS(_factoryCode, JsonConvert.SerializeObject(model), _token);
        }

        public void EditDocList(ManageDocument model)
        {
            model.Username = _username;
            _documentSRepository.UpdateDocumentS(_factoryCode, JsonConvert.SerializeObject(model), _token);
        }

        public void DeleteDocList(string id)
        {
            _documentSRepository.DeleteDocumentS(_factoryCode, id, _token);
        }

        public ReportDocumentS GetReportDocumentS(string Snumber)
        {
            return JsonConvert.DeserializeObject<ReportDocumentS>(_documentSRepository.GetDocumentSReport(_factoryCode, Snumber, _username, _token));
        }

        public void SearchDocumentsAndMODataByOrderItem(ref List<DocumentsMOData> moDatas, string orderItem, string sNumber)
        {
            moDatas = new List<DocumentsMOData>();
            moDatas = JsonConvert.DeserializeObject<List<DocumentsMOData>>(_documentSRepository.SearchDocumentsAndMODataByOrderItem(_factoryCode, sNumber, orderItem, _token));
            //if (moDatas != null && moDatas.Count > 10)
            //{
            //    moDatas = moDatas.Take(10).ToList();
            //}
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "Documents_MODatasSearch", moDatas);

        }

        public void ChangeOrderQuantity(ref List<DocumentsMOData> moDatas, string orderItem, string changeOrderQuantity)
        {
            moDatas = new List<DocumentsMOData>();
            var documentsMODatas = SessionExtentions.GetSession<List<DocumentsMOData>>(_httpContextAccessor.HttpContext.Session, "Documents_MODatasSearch");
            var changeQuantitys = SessionExtentions.GetSession<List<DocumentsMOData>>(_httpContextAccessor.HttpContext.Session, "Documents_ChangeOrderQuantity");

            if (documentsMODatas != null)
            {
                var isBox = string.IsNullOrEmpty(orderItem) ? false : documentsMODatas.FirstOrDefault(d => d.OrderItem == orderItem).isBox;
                if ((string.IsNullOrEmpty(changeOrderQuantity) && isBox) || string.IsNullOrEmpty(orderItem))
                {
                    moDatas.AddRange(documentsMODatas);
                }
                else
                {
                    if (changeQuantitys == null)
                    {
                        if (isBox)
                        {
                            documentsMODatas.ForEach(c => c.ChangeQuantity = c.PieceSet.Value * Convert.ToInt32(changeOrderQuantity));
                            documentsMODatas.ForEach(c => c.BoxChange = c.OrderItem == orderItem ? true : false);
                        }
                        else
                        {
                            documentsMODatas.ForEach(c => c.ChangeQuantity = c.OrderItem == orderItem ? Convert.ToInt32(changeOrderQuantity) : c.ChangeQuantity);

                        }

                        moDatas.AddRange(documentsMODatas);
                    }
                    else
                    {
                        if (isBox)
                        {
                            changeQuantitys.ForEach(c => c.ChangeQuantity = c.PieceSet.Value * Convert.ToInt32(changeOrderQuantity));
                            changeQuantitys.ForEach(c => c.BoxChange = c.OrderItem == orderItem ? true : false);
                        }
                        else
                        {
                            changeQuantitys.ForEach(c => c.ChangeQuantity = c.OrderItem == orderItem ? Convert.ToInt32(changeOrderQuantity) : c.ChangeQuantity);

                        }

                        moDatas.AddRange(changeQuantitys);
                    }
                }

                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "Documents_ChangeOrderQuantity", moDatas);
            }

        }

        public void BindSelectMoDatasToDocuments(ref DocumentsMOData moData, ref bool orderItemsForSave, List<string> orderItems)
        {
            var documentsMODatas = SessionExtentions.GetSession<List<DocumentsMOData>>(_httpContextAccessor.HttpContext.Session, "Documents_MODatasSearch");
            var changeQuantitys = SessionExtentions.GetSession<List<DocumentsMOData>>(_httpContextAccessor.HttpContext.Session, "Documents_ChangeOrderQuantity");

            moData = new DocumentsMOData();
            if (changeQuantitys != null)
            {
                var selectBox = changeQuantitys.Where(c => orderItems.Any(o => o == c.OrderItem) && c.isBox);
                var boxMoDatas = documentsMODatas.Where(d => orderItems.Any(o => o == d.OrderItem) && d.isBox).ToList();
                if (selectBox != null && boxMoDatas != null && boxMoDatas.Count() == 1)
                {
                    if (selectBox.Count() == 1)
                    {
                        moData = selectBox.FirstOrDefault();
                    }
                    else
                    {
                        orderItemsForSave = true;
                    }
                }
                else
                {
                    if (orderItems.Count == 1)
                    {
                        moData = changeQuantitys.FirstOrDefault(c => c.OrderItem == orderItems.First());
                    }
                    else
                    {
                        orderItemsForSave = true;
                    }
                }
            }
            else
            {
                var boxMoDatas = documentsMODatas.Where(d => orderItems.Any(o => o == d.OrderItem) && d.isBox).ToList();
                if (boxMoDatas != null)
                {
                    if (boxMoDatas.Count == 1)
                    {
                        moData = boxMoDatas.FirstOrDefault();
                    }
                    else
                    {
                        orderItemsForSave = true;
                    }
                }
                else
                {
                    if (orderItems.Count == 1)
                    {
                        moData = documentsMODatas.FirstOrDefault(d => d.OrderItem == orderItems.First());
                    }
                    else
                    {
                        orderItemsForSave = true;
                    }
                }
            }

        }

        public void SaveChangeDocuments(List<ManageDocument> model, string orderItem)
        {
            var documentsMODatas = SessionExtentions.GetSession<List<DocumentsMOData>>(_httpContextAccessor.HttpContext.Session, "Documents_MODatasSearch");
            var documentsList = new List<DocumentSlist>();
            var snumber = model.FirstOrDefault().Snumber;
            foreach (var manageDoc in model)
            {
                var tempOfDoc = new DocumentsMOData();
                tempOfDoc = documentsMODatas.FirstOrDefault(d => d.OrderItem == manageDoc.OrderItem);
                tempOfDoc.CreatedBy = string.IsNullOrEmpty(tempOfDoc.CreatedBy) ? _username : tempOfDoc.CreatedBy;
                tempOfDoc.CreatedDate = tempOfDoc.CreatedDate == null ? DateTime.Now : tempOfDoc.CreatedDate;
                tempOfDoc.UpdatedBy = _username;
                tempOfDoc.UpdatedDate = DateTime.Now;
                tempOfDoc.Hold = manageDoc.Hold;
                tempOfDoc.Cancel = manageDoc.Cancel;
                tempOfDoc.OrderQtyOld = tempOfDoc.OrderQuantity;
                tempOfDoc.OrderQtyNew = manageDoc.OrderQtyNew == null ? tempOfDoc.OrderQuantity : manageDoc.OrderQtyNew;
                tempOfDoc.Remark = !string.IsNullOrEmpty(manageDoc.Remark) ? manageDoc.Remark : tempOfDoc.Remark;

                var DocumentSlistModel = mapper.Map<DocumentsMOData, DocumentSlist>(tempOfDoc);
                DocumentSlistModel.DuedateOld = tempOfDoc.DueDate;

                if (manageDoc.DuedateNew != null)
                {
                    DocumentSlistModel.DuedateNew = Convert.ToDateTime(manageDoc.DuedateNew);
                }
                else
                {
                    DocumentSlistModel.DuedateNew = null;
                }
                DocumentSlistModel.BoxStatus = tempOfDoc.BoxType == "82" ? "" : manageDoc.PartStatus;
                DocumentSlistModel.PartStatus = tempOfDoc.BoxType == "82" ? manageDoc.PartStatus : "";

                documentsList.Add(DocumentSlistModel);
            }

            documentsList.ForEach(d => d.Snumber = snumber);
            _documentSRepository.SaveChangeDocuments(_factoryCode, JsonConvert.SerializeObject(documentsList), _token);

            #region Old save documentsList
            //var changeQuantitys = SessionExtentions.GetSession<List<DocumentsMOData>>(_httpContextAccessor.HttpContext.Session, "Documents_ChangeOrderQuantity");
            //var documentsList = new List<DocumentSlist>();
            //if (model != null)
            //{
            //    var orderItemArr = model.OrderItem.Split(',');
            //    var orderItems = String.IsNullOrEmpty(model.OrderItem) ? orderItemArr.ToList() : orderItemArr.Where(o => o != orderItem).ToList();
            //    var hasChange = changeQuantitys == null ? false : true;
            //    var mapDocumentsList = new DocumentSlist();
            //    var mapDocumentsLists = new List<DocumentSlist>();
            //    var documentsMOData = new DocumentsMOData();
            //    var allSelectMODatas = new List<DocumentsMOData>();
            //    if (orderItemArr.Count() > 1)
            //    {
            //        if (!hasChange)
            //        {
            //            documentsMOData = documentsMODatas.FirstOrDefault(d => d.OrderItem == orderItem && d.Snumber == d.Snumber);
            //            allSelectMODatas = documentsMODatas.Where(c => orderItems.Any(o => o == c.OrderItem)).ToList();
            //        }
            //        else
            //        {
            //            documentsMOData = changeQuantitys.FirstOrDefault(d => d.OrderItem == orderItem && d.Snumber == d.Snumber);
            //            allSelectMODatas = changeQuantitys.Where(c => orderItems.Any(o => o == c.OrderItem)).ToList();
            //        }

            //        #region Change EditField Documents
            //        if (documentsMOData != null)
            //        {
            //            documentsMOData.CreatedBy = string.IsNullOrEmpty(documentsMOData.CreatedBy) ? _username : documentsMOData.CreatedBy;
            //            documentsMOData.UpdatedBy = _username;
            //            documentsMOData.UpdatedDate = DateTime.Now;
            //            documentsMOData.Hold = model.Hold;
            //            documentsMOData.Cancel = model.Cancel;
            //            documentsMOData.OrderQtyOld = documentsMOData.OrderQuantity;
            //            documentsMOData.OrderQtyNew = model.OrderQtyNew;
            //            documentsMOData.Remark = model.Remark;

            //            mapDocumentsList = mapper.Map<DocumentsMOData, DocumentSlist>(documentsMOData);
            //            mapDocumentsList.DuedateOld = documentsMOData.DueDate;
            //            if (model.DuedateNew != null)
            //            {
            //                mapDocumentsList.DuedateNew = Convert.ToDateTime(model.DuedateNew);
            //            }
            //            else
            //            {
            //                mapDocumentsList.DuedateNew = null;
            //            }
            //            mapDocumentsList.BoxStatus = documentsMOData.BoxType == "82" ? "" : model.PartStatus;
            //            mapDocumentsList.PartStatus = documentsMOData.BoxType == "82" ? model.PartStatus : "";
            //            documentsList.Add(mapDocumentsList);
            //        }
            //        #endregion


            //        allSelectMODatas.ForEach(a => a.BoxStatus = a.BoxType == "82" ? "" : a.PartStatus);
            //        allSelectMODatas.ForEach(a => a.PartStatus = a.BoxType == "82" ? a.PartStatus : "");
            //        allSelectMODatas.ForEach(a => a.CreatedBy = string.IsNullOrEmpty(a.CreatedBy) ? _username : a.CreatedBy);
            //        allSelectMODatas.ForEach(a => a.UpdatedBy = _username);
            //        allSelectMODatas.ForEach(a => a.DuedateOld = a.DueDate.ToShortDateString());
            //        allSelectMODatas.ForEach(a => a.OrderQtyNew = a.ChangeQuantity);
            //        allSelectMODatas.ForEach(a => a.OrderQtyOld = a.OrderQuantity);
            //        mapDocumentsLists = mapper.Map<List<DocumentsMOData>, List<DocumentSlist>>(allSelectMODatas);
            //        if (mapDocumentsLists != null && mapDocumentsLists.Count > 0)
            //        {
            //            documentsList.AddRange(mapDocumentsLists);
            //        }
            //    }
            //    else if (orderItemArr.Count() == 1)
            //    {
            //        var documentsListSelected = new DocumentsMOData();
            //        if (changeQuantitys != null)
            //        {
            //            documentsListSelected = changeQuantitys.FirstOrDefault(d => d.OrderItem == model.OrderItem && d.Snumber == d.Snumber);
            //        }
            //        else
            //        {
            //            documentsListSelected = documentsMODatas.FirstOrDefault(d => d.OrderItem == model.OrderItem && d.Snumber == d.Snumber);
            //        }

            //        documentsListSelected.UpdatedBy = _username;
            //        documentsListSelected.UpdatedDate = DateTime.Now;
            //        documentsListSelected.Hold = model.Hold;
            //        documentsListSelected.Cancel = model.Cancel;
            //        documentsListSelected.OrderQtyOld = documentsListSelected.OrderQuantity;
            //        documentsListSelected.OrderQtyNew = model.OrderQtyNew;
            //        documentsListSelected.Remark = model.Remark;

            //        mapDocumentsList = mapper.Map<DocumentsMOData, DocumentSlist>(documentsListSelected);
            //        mapDocumentsList.DuedateOld = documentsListSelected.DueDate;

            //        if (model.DuedateNew != null)
            //        {
            //            mapDocumentsList.DuedateNew = Convert.ToDateTime(model.DuedateNew);
            //        }
            //        else
            //        {
            //            mapDocumentsList.DuedateNew = null;
            //        }
            //        mapDocumentsList.BoxStatus = documentsListSelected.BoxType == "82" ? "" : model.PartStatus;
            //        mapDocumentsList.PartStatus = documentsListSelected.BoxType == "82" ? model.PartStatus : "";
            //        documentsList.Add(mapDocumentsList);

            //    }

            //    documentsList.ForEach(d => d.Snumber = model.Snumber);
            //    _documentSRepository.SaveChangeDocuments(_factoryCode, JsonConvert.SerializeObject(documentsList), _token);

            //}
            #endregion


        }

        public void SearchReportDocumentS(ref List<DocumentSlist> documentSlists, string customerName, string so, string materialNo, string pc)
        {
            documentSlists = new List<DocumentSlist>();
            documentSlists = JsonConvert.DeserializeObject<List<DocumentSlist>>(_documentSRepository.GetDocumentSListForReportDocument(_factoryCode, materialNo, customerName, pc, so, _token));

        }
    }
}
