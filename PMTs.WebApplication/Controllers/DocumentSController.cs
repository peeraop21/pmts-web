using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.ManageMO;
using PMTs.DataAccess.ModelView.Report;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
//using iTextSharp.text.pdf;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Font = System.Drawing.Font;

namespace PMTs.WebApplication.Controllers
{
    public class DocumentSController : Controller
    {
        private readonly IDocumentSService _documentSService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IExtensionService _extensionService;
        private readonly IBomService _bomService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public DocumentSController(IDocumentSService documentSService, IExtensionService extensionService, IBomService bomService, IHttpContextAccessor _httpContextAccessor, IHostingEnvironment hostingEnvironment)
        {
            _extensionService = extensionService;
            _documentSService = documentSService;
            _bomService = bomService;
            this.httpContextAccessor = _httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "Documents_MODatasSearch", null);
            SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "Documents_ChangeOrderQuantity", null);
            return View();
        }

        #region [Main Process]
        public JsonResult CreateDocumentS()
        {
            CreateDocumentSModel res = new CreateDocumentSModel();
            res = _documentSService.CreateDocumentS();
            return Json(RenderView.RenderRazorViewToString(this, "_ChangeSoMain", res));
        }

        public JsonResult SearchDocumentS(string SNumber)
        {
            var model = _documentSService.GetDocumentS(SNumber);
            return Json(RenderView.RenderRazorViewToString(this, "_SoMainTable", model));
        }

        public JsonResult SearchOrderData(string OrderItem)
        {
            var model = _documentSService.GetOrderData(OrderItem);
            return Json(model);
        }

        public IActionResult BackTomain()
        {
            //CreateDocumentSModel res = new CreateDocumentSModel();
            //return Json(RenderView.RenderRazorViewToString(this, "_SoMain", res));
            return RedirectToAction("Index");
        }

        public JsonResult EditDocumentS(string snumber)
        {
            CreateDocumentSModel res = new CreateDocumentSModel();
            //var tmpGetDoc =   _documentSService.GetOrderData(snumber);
            var tmpGetDoc = _documentSService.GetDocumentS(snumber);
            res = tmpGetDoc;
            res.documentS = res.ldocumentS.Select(x => x).First();
            var tmpGetDocs = _documentSService.GetDocumentSList(snumber);
            res.ldocumentSlist = tmpGetDocs;
            return Json(RenderView.RenderRazorViewToString(this, "_ChangeSoMain", res));
        }
        #endregion


        #region [Change]
        public JsonResult ByDataEditDocumentList(int id, string snumber)
        {
            SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "Documents_MODatasSearch", null);
            SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "Documents_ChangeOrderQuantity", null);
            CreateDocumentSModel res = new CreateDocumentSModel();
            var tmp = _documentSService.GetDocumentSList(snumber).Where(x => x.Id == id).FirstOrDefault();
            res.documentSData = _documentSService.GetOrderData(tmp.OrderItem);
            res.documentSlist = tmp;
            return Json(res);
        }

        public JsonResult DeletChangeS(int id, string snumber)
        {
            var isSuccess = false;
            string exceptionMessage = string.Empty;
            CreateDocumentSModel models = new CreateDocumentSModel();
            try
            {
                _documentSService.DeleteDocList(id.ToString());
                var res = _documentSService.GetDocumentSList(snumber);
                models.ldocumentSlist = res;
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }

            return Json(new
            {
                IsSuccess = isSuccess,
                ExceptionMessage = exceptionMessage,
                View = RenderView.RenderRazorViewToString(this, "_ChangeSoMainTable", models)
            });
        }

        public JsonResult EditChangeS(ManageDocument model)
        {
            var isSuccess = false;
            string exceptionMessage = string.Empty;
            CreateDocumentSModel models = new CreateDocumentSModel();
            try
            {
                var xxx = model.DuedateOld.Split("/");
                var ds = xxx[2] + "-" + xxx[1] + "-" + xxx[0];
                model.DuedateOld = ds;
                _documentSService.EditDocList(model);
                var res = _documentSService.GetDocumentSList(model.Snumber);
                models.ldocumentSlist = res;
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }

            return Json(new
            {
                IsSuccess = isSuccess,
                ExceptionMessage = exceptionMessage,
                View = RenderView.RenderRazorViewToString(this, "_ChangeSoMainTable", models)
            });
        }

        public JsonResult AddChangeS(ManageDocument model)
        {
            var isSuccess = false;
            string exceptionMessage = string.Empty;
            CreateDocumentSModel models = new CreateDocumentSModel();

            try
            {
                var xxx = model.DuedateOld.Split("/");
                var ds = xxx[2] + "-" + xxx[1] + "-" + xxx[0];
                model.DuedateOld = ds;
                _documentSService.AddDocList(model);
                var res = _documentSService.GetDocumentSList(model.Snumber);
                models.ldocumentSlist = res;
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }

            return Json(new
            {
                IsSuccess = isSuccess,
                ExceptionMessage = exceptionMessage,
                View = RenderView.RenderRazorViewToString(this, "_ChangeSoMainTable", models)
            });
        }

        public JsonResult SaveChangeDocuments(List<ManageDocument> model, string orderItem)
        {
            CreateDocumentSModel models = new CreateDocumentSModel();
            var isSuccess = false;
            string exceptionMessage = string.Empty;

            try
            {
                _documentSService.SaveChangeDocuments(model, orderItem);
                //var res = _documentSService.GetDocumentSList(model.Snumber);
                //models.ldocumentSlist = res;
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }
            return Json(new
            {
                IsSuccess = isSuccess,
                ExceptionMessage = exceptionMessage,
                //View = RenderView.RenderRazorViewToString(this, "_ChangeSoMainTable", models)
            });
        }
        #endregion

        public JsonResult CheckHashOrderItem(string snumber, string orderitem)
        {
            var isSuccess = false;
            string exceptionMessage = string.Empty;
            var res = _documentSService.GetDocumentSList(snumber);
            string result = string.Empty;
            if (res != null)
            {
                var checkhasorder = res.Where(x => x.OrderItem == orderitem).FirstOrDefault();
                if (checkhasorder == null)
                {
                    result = "0";
                }
                else
                {
                    result = checkhasorder.Id.ToString();
                }

            }
            else
            {
                result = "0";
            }
            return Json(result);
        }

        public JsonResult SearchDocumentsAndMODataByOrderItem(string orderItem, string sNumber)
        {
            var isSuccess = false;
            string exceptionMessage = string.Empty;
            string result = string.Empty;
            var moDatas = new List<DocumentsMOData>();

            try
            {
                SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "Documents_MODatasSearch", null);
                SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "Documents_ChangeOrderQuantity", null);
                _documentSService.SearchDocumentsAndMODataByOrderItem(ref moDatas, orderItem, sNumber);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                moDatas = new List<DocumentsMOData>();
                exceptionMessage = ex.Message;
            }

            return Json(new
            {
                IsSuccess = isSuccess,
                ExceptionMessage = exceptionMessage,
                MOCount = moDatas.Count,
                ViewTable = RenderView.RenderRazorViewToString(this, "_MODatasTableSelectorTable", moDatas),
                ModelDocument = moDatas.Count >= 1 ? JsonConvert.SerializeObject(moDatas.FirstOrDefault()) : string.Empty,
                MODatas = moDatas.Count >= 1 ? JsonConvert.SerializeObject(moDatas.ToList()) : string.Empty,
            });
        }

        public JsonResult CalculateOrderQuantityDocuments(string orderItem, string changeOrderQuantity)
        {
            var isSuccess = false;
            string exceptionMessage = string.Empty;
            var moDatas = new List<DocumentsMOData>();

            try
            {
                _documentSService.ChangeOrderQuantity(ref moDatas, orderItem, changeOrderQuantity);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                moDatas = new List<DocumentsMOData>();
                exceptionMessage = ex.Message;
            }

            return Json(new
            {
                IsSuccess = isSuccess,
                ExceptionMessage = exceptionMessage,
                LastChange = orderItem,
                ViewTable = RenderView.RenderRazorViewToString(this, "_MODatasTableSelectorTable", moDatas)
            });
        }

        public JsonResult SelectMOdatasToDocuments(List<string> orderItems)
        {
            var isSuccess = false;
            var orderItemsForSave = false;
            string exceptionMessage = string.Empty;
            var moData = new DocumentsMOData();

            try
            {
                _documentSService.BindSelectMoDatasToDocuments(ref moData, ref orderItemsForSave, orderItems);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                moData = new DocumentsMOData();
                exceptionMessage = ex.Message;
            }

            return Json(new
            {
                IsSuccess = isSuccess,
                CountOfOrderItems = orderItems.Count,
                OrderItemsForSave = orderItemsForSave,
                ExceptionMessage = exceptionMessage,
                BoxModel = JsonConvert.SerializeObject(moData)
            });
        }

        #region [Print]
        [SessionTimeout]
        public async Task<IActionResult> DocumentSExportExcel(string req)
        {
            //ProductCatalogsSearch data = new ProductCatalogsSearch();
            //  string normal = "";//ConvertHexToString(req, System.Text.Encoding.Unicode);
            //  data = JsonConvert.DeserializeObject<ProductCatalogsSearch>(normal);
            // var myBUs = new DataTable(); //_productCatalog.CreateDynamicDataTable(data); ;//await _context.BusinessUnits.ToListAsync();
            // above code loads the data using LINQ with EF (query of table), you can substitute this with any data source.


            //Create a stream of .xlsx file contained within my project using reflection
            //Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PMTs.WebApplication.Config.log4net.config");

            //EPPlusTest = Namespace/Project
            //templates = folder
            //VendorTemplate.xlsx = file

            //ExcelPackage has a constructor that only requires a stream.
            // ExcelPackage pck = new OfficeOpenXml.ExcelPackage(stream);

            //FileInfo template = new FileInfo(Server.MapPath(@"Xls/Template.xlsx"));

            var data = _documentSService.GetReportDocumentS(req);
            string groupCom = _bomService.GetAllGroupCompany();

            string excelNames = string.Empty;
            string excelNamespath = string.Empty;
            FileInfo template;
            if (groupCom == "4")
            {
                template = new FileInfo("D:\\DocumentSTemplate\\NewTemplateDocsDyna.xlsx");
            }
            else
            {
                //template = new FileInfo("D:\\DocumentSTemplate\\TemplateDocsx.xlsx");
                template = new FileInfo("D:\\DocumentSTemplate\\NewTemplateDocsx.xlsx");
            }

            using (var package = new ExcelPackage(template))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var workbook = package.Workbook;

                //*** Sheet 1
                var worksheet = workbook.Worksheets.First();

                // Rows1
                int rowstart = 8;
                int autorow = 1;
                foreach (var item in data.reportDocumentSlists)
                {
                    var masterdata = _bomService.GetMasterdataByMaterial(item.MaterialNo, item.FactoryCode);
                    var mattype = masterdata != null ? masterdata.MaterialType : null;
                    worksheet.Cells["B" + rowstart.ToString()].Value = autorow;
                    worksheet.Cells["B" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["C" + rowstart.ToString()].Value = item.OrderItem.Trim();
                    worksheet.Cells["C" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["D" + rowstart.ToString()].Value = item.Pc;
                    worksheet.Cells["D" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["E" + rowstart.ToString()].Value = item.MaterialNo.Trim();
                    worksheet.Cells["E" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    if (groupCom == "4")
                    {
                        worksheet.Cells["F" + rowstart.ToString()].Value = item.CustomerName.Trim();
                        worksheet.Cells["F" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["G" + rowstart.ToString()].Value = item.Flute.Trim();
                        worksheet.Cells["G" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        string dueOld = "";
                        try { dueOld = item.DuedateOld.Value.ToString("dd/MM/yyyy"); } catch { dueOld = ""; }
                        worksheet.Cells["H" + rowstart.ToString()].Value = dueOld;
                        worksheet.Cells["H" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        string dueNew = "";
                        try { dueNew = item.DuedateNew.Value.ToString("dd/MM/yyyy") == item.DuedateOld.Value.ToString("dd/MM/yyyy") ? "" : item.DuedateNew.Value.ToString("dd/MM/yyyy"); } catch { dueNew = ""; }
                        worksheet.Cells["I" + rowstart.ToString()].Value = dueNew;
                        worksheet.Cells["I" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        worksheet.Cells["J" + rowstart.ToString()].Value = string.Format("{0:N0}", item.OrderQtyOld);
                        worksheet.Cells["K" + rowstart.ToString()].Value = item.OrderQtyNew == item.OrderQtyOld ? null : string.Format("{0:N0}", item.OrderQtyNew);

                        if (mattype == "82")
                        {
                            string dueVendorOld = "";
                            try { dueVendorOld = item.DuedateOld.Value.AddDays(-1).ToString("dd/MM/yyyy"); } catch { dueVendorOld = ""; }
                            worksheet.Cells["L" + rowstart.ToString()].Value = dueVendorOld;
                            worksheet.Cells["L" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            string dueVendorNew = "";
                            try { dueVendorNew = item.DuedateNew.Value.ToString("dd/MM/yyyy") == item.DuedateOld.Value.ToString("dd/MM/yyyy") ? "" : item.DuedateNew.Value.AddDays(-1).ToString("dd/MM/yyyy"); } catch { dueVendorNew = ""; }
                            worksheet.Cells["M" + rowstart.ToString()].Value = dueVendorNew;
                            worksheet.Cells["M" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        }
                        worksheet.Cells["N" + rowstart.ToString()].Value = item.Cancel == true ? "/" : "";
                        worksheet.Cells["N" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["O" + rowstart.ToString()].Value = item.Hold == true ? "/" : "";
                        worksheet.Cells["O" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["P" + rowstart.ToString()].Value = item.BoxStatus;
                        worksheet.Cells["Q" + rowstart.ToString()].Value = item.PartStatus;
                        worksheet.Cells["T" + rowstart.ToString()].Value = _documentSService.GetShortNameFacOfOutsourceByOrderItem(item.OrderItem.Trim());
                        worksheet.Cells["U" + rowstart.ToString()].Value = string.IsNullOrEmpty(item.Process) ? item.Process :
                            item.Process.Length > 30 ? item.Process.Substring(0, 30) : item.Process;
                        worksheet.Cells["V" + rowstart.ToString()].Value = string.IsNullOrEmpty(item.Remark) ? item.Remark :
                            item.Remark.Length > 40 ? item.Remark.Substring(0, 40) : item.Remark;
                    }
                    else
                    {

                        worksheet.Cells["F" + rowstart.ToString()].Value = item.Flute.Trim();
                        worksheet.Cells["F" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        string dueOld = "";
                        try { dueOld = item.DuedateOld.Value.ToString("dd/MM/yyyy"); } catch { dueOld = ""; }
                        worksheet.Cells["G" + rowstart.ToString()].Value = dueOld;
                        worksheet.Cells["G" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        string dueNew = "";
                        try { dueNew = item.DuedateNew.Value.ToString("dd/MM/yyyy") == item.DuedateOld.Value.ToString("dd/MM/yyyy") ? "" : item.DuedateNew.Value.ToString("dd/MM/yyyy"); } catch { dueNew = ""; }
                        worksheet.Cells["H" + rowstart.ToString()].Value = dueNew;
                        worksheet.Cells["H" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        worksheet.Cells["I" + rowstart.ToString()].Value = string.Format("{0:N0}", item.OrderQtyOld);
                        worksheet.Cells["J" + rowstart.ToString()].Value = item.OrderQtyNew == item.OrderQtyOld ? null : string.Format("{0:N0}", item.OrderQtyNew);

                        if (mattype == "82")
                        {
                            string dueVendorOld = "";
                            try { dueVendorOld = item.DuedateOld.Value.AddDays(-1).ToString("dd/MM/yyyy"); } catch { dueVendorOld = ""; }
                            worksheet.Cells["K" + rowstart.ToString()].Value = dueVendorOld;
                            worksheet.Cells["K" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            string dueVendorNew = "";
                            try { dueVendorNew = item.DuedateNew.Value.ToString("dd/MM/yyyy") == item.DuedateOld.Value.ToString("dd/MM/yyyy") ? "" : item.DuedateNew.Value.AddDays(-1).ToString("dd/MM/yyyy"); } catch { dueVendorNew = ""; }
                            worksheet.Cells["L" + rowstart.ToString()].Value = dueVendorNew;
                            worksheet.Cells["L" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }


                        worksheet.Cells["M" + rowstart.ToString()].Value = item.Cancel == true ? "/" : "";
                        worksheet.Cells["M" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["N" + rowstart.ToString()].Value = item.Hold == true ? "/" : "";
                        worksheet.Cells["N" + rowstart.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["O" + rowstart.ToString()].Value = item.BoxStatus;
                        worksheet.Cells["P" + rowstart.ToString()].Value = item.PartStatus;
                        worksheet.Cells["S" + rowstart.ToString()].Value = _documentSService.GetShortNameFacOfOutsourceByOrderItem(item.OrderItem.Trim());
                        worksheet.Cells["T" + rowstart.ToString()].Value = string.IsNullOrEmpty(item.Process) ? item.Process :
                            item.Process.Length > 30 ? item.Process.Substring(0, 30) : item.Process;
                        worksheet.Cells["U" + rowstart.ToString()].Value = string.IsNullOrEmpty(item.Remark) ? item.Remark :
                            item.Remark.Length > 40 ? item.Remark.Substring(0, 40) : item.Remark;

                    }

                    rowstart++;
                    autorow++;
                }


                // Rows2
                var companyName = data.CompanyTh.Replace("ห้างหุ้นส่วนจำกัด", "").Replace("บริษัท", "");
                worksheet.Cells["I1"].Value = companyName;
                worksheet.Cells["C4"].Value = data.reportDocumentSlists != null ? data.reportDocumentSlists.Select(x => x.CustomerName).FirstOrDefault() : "";
                worksheet.Cells["C4:G4"].Merge = true;
                worksheet.Cells["C4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                if (groupCom != "4")
                {
                    worksheet.Cells["I2"].Value = data.CompanyEn;
                }

                worksheet.Cells["T1"].Value = "เลขที่ " + data.Docsnumber;
                if (groupCom == "4")
                {
                    worksheet.Cells["B24"].Value = "ผู้เสนอ " + data.UserCreate + " บริการลูกค้า";
                    worksheet.Cells["B32"].Value = "รหัสเอกสาร " + data.SDocName;
                    worksheet.Cells["T33"].Value = "วันที่เริ่มใช้ " + data.SDocDate;
                    worksheet.Cells["C25"].Value = "วันที่ " + DateTime.Now.ToString("dd/MM/yyyy");
                }
                else
                {
                    worksheet.Cells["B23"].Value = "ผู้เสนอ " + data.UserCreate;
                    worksheet.Cells["B26"].Value = "รหัสเอกสาร " + data.SDocName;
                    worksheet.Cells["T27"].Value = "วันที่เริ่มใช้ " + data.SDocDate;
                    worksheet.Cells["C24"].Value = "วันที่ " + DateTime.Now.ToString("dd/MM/yyyy");
                }



                excelNames = $"DocumentS-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                excelNamespath = $"D:\\DocumentSTemplate\\DocS-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                package.SaveAs(new FileInfo(excelNamespath));

            }



            var byteArray = System.IO.File.ReadAllBytes(excelNamespath);
            var streamer = new MemoryStream(byteArray);
            System.IO.File.Delete(excelNamespath);
            return File(streamer, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelNames);
            //    return File(streamer, "application/pdf", excelNames);
        }
        #endregion
        public async Task<IActionResult> ExportDocumentsPDF(string req)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                var data = _documentSService.GetReportDocumentS(req);
                string groupCom = _bomService.GetAllGroupCompany();

                //var SessionUser = SessionExtentions.GetSession<UserLoginModel>(HttpContext.Session, "UserSessionModel");
                //_raformDataViewModel.RAFormData = _rAForm16DataService.GetRaform16DataById(SessionUser.Usename, taskForm, SessionUser.RoleName, typeUserAction, SessionUser.Token);

                #region [Setup Font]
                System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
                Encoding.RegisterProvider(ppp);
                //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
                string targetPath = "pdfFont\\angsanaupc.TTF";

                string fontPath = Path.Combine(_hostingEnvironment.WebRootPath, targetPath);

                var image = iTextSharp.text.Image.GetInstance(_hostingEnvironment.WebRootPath + "\\pdfFont\\SCGLogo.jpg");
                image.ScaleAbsolute(100f, 80f);

                // string fontPath = Path.Combine("D", targetPath);

                //BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                //BaseFont bf = BaseFont.CreateFont(@"D:\HyperSoft_Project\ratask\RATask.Web\wwwroot\FontPdf\ANGSA.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font fontHeader = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font fontHeader2 = new iTextSharp.text.Font(bf, 14, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font fontNormal = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font fontNormal8 = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font fontUnderLine = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.UNDERLINE);
                iTextSharp.text.Font fontUnderLine10 = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.UNDERLINE);
                iTextSharp.text.Font fontUnderLine8 = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.UNDERLINE);
                iTextSharp.text.Font fontBold = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD);
                //Font fontCenterLine = new Font(bf, 12, Font.UNDEFINED); 
                iTextSharp.text.Font fontItalic = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.ITALIC);
                iTextSharp.text.Font fontItalic10 = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.ITALIC);
                iTextSharp.text.Font fontStrike = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.STRIKETHRU);

                #endregion

                Document document = new Document();
                PdfWriter.GetInstance(document, ms);// new System.IO.FileStream("dddd.pdf", System.IO.FileMode.Create));
                document.SetPageSize(PageSize.A4.Rotate());
                document.SetMargins(20f, 20f, 5f, 5f);
                document.Open();


                //bool statusNextPage = false;
                //int indexOfTask = 0;

                //groupCom = "4";
                if (groupCom == "4")
                {

                    int indexloop = 0;
                    int caseDiffTrans = 0;
                    int totalTbl = data.reportDocumentSlists.Count;
                    decimal calpage = (decimal)(totalTbl - caseDiffTrans) / 8;
                    int pageNextTotal = (int)Math.Ceiling(calpage);
                    bool statusNextPage = false;
                    int allCounterLoop = 0;
                    if (pageNextTotal == 0)
                    {
                        #region [Header]

                        document.Add(new Paragraph(new Chunk("          ", fontNormal)));
                        float[] columnWidths1 = { 3, 1, 5, 3 };
                        PdfPTable tableHeader = new PdfPTable(columnWidths1);
                        tableHeader.WidthPercentage = 100f;


                        PdfPCell h1 = new PdfPCell(new Phrase(new Chunk("เอกสารควบคุม", fontUnderLine)));
                        h1.HorizontalAlignment = 0;
                        //h1.Rowspan = 2;
                        h1.BorderColor = BaseColor.White;
                        h1.Border = 1;


                        var cimage = new Chunk(image, 0, 0, false);
                        var h2 = new PdfPCell { PaddingLeft = 0, PaddingTop = 0, PaddingBottom = 0, PaddingRight = 2 };
                        h2.AddElement(cimage);
                        h2.HorizontalAlignment = Element.ALIGN_LEFT;
                        h2.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //h2.FixedHeight = 20;

                        h2.BorderColor = BaseColor.White;
                        h2.Border = 0;

                        //PdfPCell h2 = new PdfPCell(new Phrase(new Chunk(image, 0, 0)));
                        //h2.HorizontalAlignment = 2;
                        //h2.Rowspan = 2;
                        //h2.BorderColor = BaseColor.White;
                        //h2.Border = 0;
                        var h2t = new Chunk("           ใบแจ้งเปลี่ยนแปลง S/O", fontHeader);
                        var h2text = new Phrase(new Chunk(data.CompanyTh + "\n" + data.CompanyEn, fontHeader2));
                        var h3 = new PdfPCell { PaddingLeft = 50, PaddingTop = 10, PaddingBottom = 0, PaddingRight = 2 };
                        h3.AddElement(h2text);
                        h3.AddElement(h2t);
                        //h3.HorizontalAlignment = 0;
                        //h3.VerticalAlignment = 0;

                        //h3.Rowspan = 2;
                        h3.BorderColor = BaseColor.White;
                        h3.Border = 0;

                        PdfPCell h4 = new PdfPCell(new Phrase(new Chunk("เลขที่ " + data.Docsnumber, fontNormal)));
                        h4.HorizontalAlignment = 2;
                        //h4.Rowspan = 2;
                        h4.BorderColor = BaseColor.White;
                        h4.Border = 0;

                        tableHeader.AddCell(h1);
                        tableHeader.AddCell(h2);
                        tableHeader.AddCell(h3);
                        tableHeader.AddCell(h4);

                        document.Add(tableHeader);


                        //Paragraph pHeader1 = new Paragraph(new Chunk("ใบแจ้งเปลี่ยนแปลง S/O", fontHeader));
                        //pHeader1.Alignment = Element.ALIGN_CENTER;
                        //Paragraph pHeader2 = new Paragraph(new Chunk("ลูกค้า" + data.reportDocumentSlists != null ? data.reportDocumentSlists.Select(x => x.CustomerName).FirstOrDefault() : "", fontHeader2));

                        Phrase pCustomer = new Phrase();
                        string customer = string.Empty;
                        if (data.reportDocumentSlists.Count == 0)
                        {
                            customer = "ลูกค้า ________________________________________________________";
                            var c1 = new Chunk(customer, fontNormal);
                            pCustomer.Add(c1);
                        }
                        else
                        {
                            //createuser = "ผู้เสนอ " + data.UserCreate + " บริการลูกค้า";
                            var c0 = new Chunk("ลูกค้า ", fontNormal);
                            var c1 = new Chunk(data.reportDocumentSlists.Select(x => x.CustomerName).FirstOrDefault(), fontUnderLine);

                            pCustomer.Add(c0);
                            pCustomer.Add(c1);

                            //var c1 = new Chunk(createuser, fontNormal);
                            //pCreate.Add(c1);
                        }

                        Paragraph pHeader2 = new Paragraph(pCustomer);
                        pHeader2.Alignment = Element.ALIGN_LEFT;

                        //document.Add(pHeader1);
                        document.Add(pHeader2);

                        #endregion

                        #region [Table Data]
                        document.Add(new Paragraph(new Chunk("          ", fontNormal)));
                        float[] columnWidths2 = { 1, 3, 3, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 3, 4, 4 };
                        PdfPTable tableData = new PdfPTable(columnWidths2);
                        tableData.WidthPercentage = 100f;

                        PdfPCell T1 = new PdfPCell(new Phrase(new Chunk("No", fontNormal)));
                        T1.HorizontalAlignment = 1;
                        T1.Rowspan = 2;

                        PdfPCell T2 = new PdfPCell(new Phrase(new Chunk("S/O", fontNormal)));
                        T2.HorizontalAlignment = 1;
                        T2.Rowspan = 2;

                        PdfPCell T3 = new PdfPCell(new Phrase(new Chunk("P/C", fontNormal)));
                        T3.HorizontalAlignment = 1;
                        T3.Rowspan = 2;

                        PdfPCell T4 = new PdfPCell(new Phrase(new Chunk("Material No", fontNormal)));
                        T4.HorizontalAlignment = 1;
                        T4.Rowspan = 2;

                        PdfPCell T5 = new PdfPCell(new Phrase(new Chunk("ชื่อสินค้า", fontNormal)));
                        T5.HorizontalAlignment = 1;
                        T5.Rowspan = 2;

                        PdfPCell T6 = new PdfPCell(new Phrase(new Chunk("ลอน", fontNormal)));
                        T6.HorizontalAlignment = 1;
                        T6.Rowspan = 2;

                        PdfPCell T7 = new PdfPCell(new Phrase(new Chunk("กำหนดส่ง	", fontNormal)));
                        T7.HorizontalAlignment = 1;
                        T7.Colspan = 2;

                        PdfPCell T8 = new PdfPCell(new Phrase(new Chunk("จำนวนสั่ง	", fontNormal)));
                        T8.HorizontalAlignment = 1;
                        T8.Colspan = 2;

                        PdfPCell T9 = new PdfPCell(new Phrase(new Chunk("ยกเลิก", fontNormal)));
                        T9.HorizontalAlignment = 1;
                        T9.Rowspan = 2;

                        PdfPCell T10 = new PdfPCell(new Phrase(new Chunk("Hold", fontNormal)));
                        T10.HorizontalAlignment = 1;
                        T10.Rowspan = 2;

                        PdfPCell T11 = new PdfPCell(new Phrase(new Chunk("สถานะกล่อง", fontNormal)));
                        T11.HorizontalAlignment = 1;
                        T11.Rowspan = 2;

                        PdfPCell T12 = new PdfPCell(new Phrase(new Chunk("สถานะ\nส่วนประกอบ", fontNormal)));
                        T12.HorizontalAlignment = 1;
                        T12.Rowspan = 2;

                        PdfPCell T13 = new PdfPCell(new Phrase(new Chunk("Process", fontNormal)));
                        T13.HorizontalAlignment = 1;
                        T13.Rowspan = 2;

                        PdfPCell T14 = new PdfPCell(new Phrase(new Chunk("หมายเหตุ", fontNormal)));
                        T14.HorizontalAlignment = 1;
                        T14.Rowspan = 2;

                        tableData.AddCell(T1);
                        tableData.AddCell(T2);
                        tableData.AddCell(T3);
                        tableData.AddCell(T4);
                        tableData.AddCell(T5);
                        tableData.AddCell(T6);
                        tableData.AddCell(T7);
                        tableData.AddCell(T8);
                        tableData.AddCell(T9);
                        tableData.AddCell(T10);
                        tableData.AddCell(T11);
                        tableData.AddCell(T12);
                        tableData.AddCell(T13);
                        tableData.AddCell(T14);

                        PdfPCell T15 = new PdfPCell(new Phrase(new Chunk("เดิม", fontNormal)));
                        T15.HorizontalAlignment = 1;
                        //T15.Rowspan = 2;

                        PdfPCell T16 = new PdfPCell(new Phrase(new Chunk("ใหม่", fontNormal)));
                        T16.HorizontalAlignment = 1;
                        //T16.Rowspan = 2;

                        PdfPCell T17 = new PdfPCell(new Phrase(new Chunk("เดิม", fontNormal)));
                        T17.HorizontalAlignment = 1;
                        //T17.Rowspan = 2;

                        PdfPCell T18 = new PdfPCell(new Phrase(new Chunk("ใหม่", fontNormal)));
                        T18.HorizontalAlignment = 1;
                        //T18.Rowspan = 2;

                        tableData.AddCell(T15);
                        tableData.AddCell(T16);
                        tableData.AddCell(T17);
                        tableData.AddCell(T18);

                        for (int i = 1; i <= 5; i++)
                        {
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                        }

                        document.Add(tableData);
                        #endregion

                        #region [Footer]

                        document.Add(new Paragraph(new Chunk("          ", fontNormal)));
                        float[] columnWidths3 = { 3, 3, 3, 3 };
                        PdfPTable tabledFooter = new PdfPTable(columnWidths3);
                        tabledFooter.WidthPercentage = 100f;
                        tabledFooter.DefaultCell.Border = 0;

                        Phrase pCreate = new Phrase();
                        string createuser = string.Empty;
                        if (string.IsNullOrEmpty(data.UserCreate))
                        {
                            createuser = "ผู้เสนอ ____________________________บริการลูกค้า";
                            var c1 = new Chunk(createuser, fontNormal);
                            pCreate.Add(c1);
                        }
                        else
                        {
                            //createuser = "ผู้เสนอ " + data.UserCreate + " บริการลูกค้า";
                            var c0 = new Chunk("ผู้เสนอ ", fontNormal);
                            var c1 = new Chunk(data.UserCreate, fontUnderLine);
                            var c2 = new Chunk(" บริการลูกค้า", fontNormal);
                            pCreate.Add(c0);
                            pCreate.Add(c1);
                            pCreate.Add(c2);
                            //var c1 = new Chunk(createuser, fontNormal);
                            //pCreate.Add(c1);
                        }

                        PdfPCell F1 = new PdfPCell(pCreate);
                        F1.HorizontalAlignment = 0;
                        F1.BorderColor = BaseColor.White;
                        F1.Border = 0;
                        PdfPCell F2 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F2.HorizontalAlignment = 0;
                        F2.BorderColor = BaseColor.White;
                        F2.Border = 0;
                        PdfPCell F3 = new PdfPCell(new Phrase(new Chunk("ผ.วางแผน ...............................................", fontNormal)));
                        F3.HorizontalAlignment = 0;
                        F3.BorderColor = BaseColor.White;
                        F3.Border = 0;
                        PdfPCell F4 = new PdfPCell(new Phrase(new Chunk("หน่วยงานพิมพ์ .............................................", fontNormal)));
                        F4.HorizontalAlignment = 0;
                        F4.BorderColor = BaseColor.White;
                        F4.Border = 0;

                        tabledFooter.AddCell(F1);
                        tabledFooter.AddCell(F2);
                        tabledFooter.AddCell(F3);
                        tabledFooter.AddCell(F4);

                        PdfPCell F11 = new PdfPCell(new Phrase(new Chunk("วันที่ " + DateTime.Now.ToString("dd/MM/yyyy"), fontNormal)));
                        F11.HorizontalAlignment = 0;
                        F11.BorderColor = BaseColor.White;
                        F11.Border = 0;
                        PdfPCell F12 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F12.HorizontalAlignment = 0;
                        F12.BorderColor = BaseColor.White;
                        F12.Border = 0;
                        PdfPCell F13 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                        F13.HorizontalAlignment = 0;
                        F13.BorderColor = BaseColor.White;
                        F13.Border = 0;
                        PdfPCell F14 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                        F14.HorizontalAlignment = 0;
                        F14.BorderColor = BaseColor.White;
                        F14.Border = 0;

                        tabledFooter.AddCell(F11);
                        tabledFooter.AddCell(F12);
                        tabledFooter.AddCell(F13);
                        tabledFooter.AddCell(F14);

                        tabledFooter.AddCell(F12);
                        tabledFooter.AddCell(F12);
                        tabledFooter.AddCell(F12);
                        tabledFooter.AddCell(F12);

                        PdfPCell F21 = new PdfPCell(new Phrase(new Chunk("หน่วยงานสำเร็จรูป .............................................", fontNormal)));
                        F21.HorizontalAlignment = 0;
                        F21.BorderColor = BaseColor.White;
                        F21.Border = 0;
                        PdfPCell F22 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F22.HorizontalAlignment = 0;
                        F22.BorderColor = BaseColor.White;
                        F22.Border = 0;
                        PdfPCell F23 = new PdfPCell(new Phrase(new Chunk("ผ.ผลิตแผ่นลูกฟูก .............................................", fontNormal)));
                        F23.HorizontalAlignment = 0;
                        F23.BorderColor = BaseColor.White;
                        F23.Border = 0;
                        PdfPCell F24 = new PdfPCell(new Phrase(new Chunk("ผ.คลังสินค้า/ขนส่ง .............................................", fontNormal)));
                        F24.HorizontalAlignment = 0;
                        F24.BorderColor = BaseColor.White;
                        F24.Border = 0;

                        tabledFooter.AddCell(F21);
                        tabledFooter.AddCell(F22);
                        tabledFooter.AddCell(F23);
                        tabledFooter.AddCell(F24);

                        PdfPCell F31 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                        F31.HorizontalAlignment = 0;
                        F31.BorderColor = BaseColor.White;
                        F31.Border = 0;
                        PdfPCell F32 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F32.HorizontalAlignment = 0;
                        F32.BorderColor = BaseColor.White;
                        F32.Border = 0;
                        PdfPCell F33 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                        F33.HorizontalAlignment = 0;
                        F33.BorderColor = BaseColor.White;
                        F33.Border = 0;
                        PdfPCell F34 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                        F34.HorizontalAlignment = 0;
                        F34.BorderColor = BaseColor.White;
                        F34.Border = 0;

                        tabledFooter.AddCell(F31);
                        tabledFooter.AddCell(F32);
                        tabledFooter.AddCell(F33);
                        tabledFooter.AddCell(F34);

                        tabledFooter.AddCell(F12);
                        tabledFooter.AddCell(F12);
                        tabledFooter.AddCell(F12);
                        tabledFooter.AddCell(F12);

                        PdfPCell F41 = new PdfPCell(new Phrase(new Chunk("รหัสเอกสาร " + data.SDocName, fontNormal)));
                        F41.HorizontalAlignment = 0;
                        F41.BorderColor = BaseColor.White;
                        F41.Border = 0;
                        PdfPCell F42 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F42.HorizontalAlignment = 0;
                        F42.BorderColor = BaseColor.White;
                        F42.Border = 0;
                        PdfPCell F43 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F43.HorizontalAlignment = 0;
                        F43.BorderColor = BaseColor.White;
                        F43.Border = 0;
                        PdfPCell F44 = new PdfPCell(new Phrase(new Chunk("แก้ไขครั้งที่  00", fontNormal)));
                        F44.HorizontalAlignment = 0;
                        F44.BorderColor = BaseColor.White;
                        F44.Border = 0;

                        tabledFooter.AddCell(F41);
                        tabledFooter.AddCell(F42);
                        tabledFooter.AddCell(F43);
                        tabledFooter.AddCell(F44);

                        PdfPCell F51 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F51.HorizontalAlignment = 0;
                        F51.BorderColor = BaseColor.White;
                        F51.Border = 0;
                        PdfPCell F52 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F52.HorizontalAlignment = 0;
                        F52.BorderColor = BaseColor.White;
                        F52.Border = 0;
                        PdfPCell F53 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F53.HorizontalAlignment = 0;
                        F53.BorderColor = BaseColor.White;
                        F53.Border = 0;
                        PdfPCell F54 = new PdfPCell(new Phrase(new Chunk("วันที่เริ่มใช้ " + data.SDocDate, fontNormal)));
                        F54.HorizontalAlignment = 0;
                        F54.BorderColor = BaseColor.White;
                        F54.Border = 0;

                        tabledFooter.AddCell(F51);
                        tabledFooter.AddCell(F52);
                        tabledFooter.AddCell(F53);
                        tabledFooter.AddCell(F54);

                        document.Add(tabledFooter);



                        #endregion

                    }
                    else
                    {

                        for (int j = 1; j <= pageNextTotal; j++)
                        {
                            if (statusNextPage)
                            {
                                document.SetPageSize(PageSize.A4.Rotate());
                                document.SetMargins(20f, 20f, 5f, 5f);
                                document.NewPage();
                            }

                            #region [Header]

                            document.Add(new Paragraph(new Chunk("          ", fontNormal)));
                            float[] columnWidths1 = { 3, 1, 5, 3 };
                            PdfPTable tableHeader = new PdfPTable(columnWidths1);
                            tableHeader.WidthPercentage = 100f;


                            PdfPCell h1 = new PdfPCell(new Phrase(new Chunk("เอกสารควบคุม", fontUnderLine)));
                            h1.HorizontalAlignment = 0;
                            //h1.Rowspan = 2;
                            h1.BorderColor = BaseColor.White;
                            h1.Border = 0;


                            var cimage = new Chunk(image, 0, 0, false);
                            var h2 = new PdfPCell { PaddingLeft = 0, PaddingTop = 0, PaddingBottom = 0, PaddingRight = 2 };
                            h2.AddElement(cimage);
                            h2.HorizontalAlignment = Element.ALIGN_LEFT;
                            h2.VerticalAlignment = Element.ALIGN_MIDDLE;
                            //h2.FixedHeight = 20;

                            h2.BorderColor = BaseColor.White;
                            h2.Border = 0;

                            //PdfPCell h2 = new PdfPCell(new Phrase(new Chunk(image, 0, 0)));
                            //h2.HorizontalAlignment = 2;
                            //h2.Rowspan = 2;
                            //h2.BorderColor = BaseColor.White;
                            //h2.Border = 0;
                            var h2t = new Chunk("           ใบแจ้งเปลี่ยนแปลง S/O", fontHeader);
                            var h2text = new Phrase(new Chunk(data.CompanyTh + "\n" + data.CompanyEn, fontHeader2));
                            var h3 = new PdfPCell { PaddingLeft = 50, PaddingTop = 10, PaddingBottom = 0, PaddingRight = 2 };
                            h3.AddElement(h2text);
                            h3.AddElement(h2t);
                            //h3.HorizontalAlignment = 0;
                            //h3.VerticalAlignment = 0;

                            //h3.Rowspan = 2;
                            h3.BorderColor = BaseColor.White;
                            h3.Border = 0;

                            PdfPCell h4 = new PdfPCell(new Phrase(new Chunk("เลขที่ " + data.Docsnumber, fontNormal)));
                            h4.HorizontalAlignment = 2;
                            //h4.Rowspan = 2;
                            h4.BorderColor = BaseColor.White;
                            h4.Border = 0;

                            tableHeader.AddCell(h1);
                            tableHeader.AddCell(h2);
                            tableHeader.AddCell(h3);
                            tableHeader.AddCell(h4);

                            document.Add(tableHeader);


                            //Paragraph pHeader1 = new Paragraph(new Chunk("ใบแจ้งเปลี่ยนแปลง S/O", fontHeader));
                            //pHeader1.Alignment = Element.ALIGN_CENTER;
                            //Paragraph pHeader2 = new Paragraph(new Chunk("ลูกค้า" + data.reportDocumentSlists != null ? data.reportDocumentSlists.Select(x => x.CustomerName).FirstOrDefault() : "", fontHeader2));

                            Phrase pCustomer = new Phrase();
                            string customer = string.Empty;
                            if (data.reportDocumentSlists.Count == 0)
                            {
                                customer = "ลูกค้า ________________________________________________________";
                                var c1 = new Chunk(customer, fontNormal);
                                pCustomer.Add(c1);
                            }
                            else
                            {
                                //createuser = "ผู้เสนอ " + data.UserCreate + " บริการลูกค้า";
                                var c0 = new Chunk("ลูกค้า ", fontNormal);
                                var c1 = new Chunk(data.reportDocumentSlists.Select(x => x.CustomerName).FirstOrDefault(), fontUnderLine);

                                pCustomer.Add(c0);
                                pCustomer.Add(c1);

                                //var c1 = new Chunk(createuser, fontNormal);
                                //pCreate.Add(c1);
                            }

                            Paragraph pHeader2 = new Paragraph(pCustomer);
                            pHeader2.Alignment = Element.ALIGN_LEFT;

                            //document.Add(pHeader1);
                            document.Add(pHeader2);

                            #endregion

                            #region [Table Data]
                            document.Add(new Paragraph(new Chunk("          ", fontNormal)));
                            float[] columnWidths2 = { 1, 3, 3, 3, 5, 2, 3, 3, 2, 2, 2, 2, 3, 3, 4, 4 };
                            PdfPTable tableData = new PdfPTable(columnWidths2);
                            tableData.WidthPercentage = 100f;

                            PdfPCell T1 = new PdfPCell(new Phrase(new Chunk("No", fontNormal)));
                            T1.HorizontalAlignment = 1;
                            T1.Rowspan = 2;

                            PdfPCell T2 = new PdfPCell(new Phrase(new Chunk("S/O", fontNormal)));
                            T2.HorizontalAlignment = 1;
                            T2.Rowspan = 2;

                            PdfPCell T3 = new PdfPCell(new Phrase(new Chunk("P/C", fontNormal)));
                            T3.HorizontalAlignment = 1;
                            T3.Rowspan = 2;

                            PdfPCell T4 = new PdfPCell(new Phrase(new Chunk("Material No", fontNormal)));
                            T4.HorizontalAlignment = 1;
                            T4.Rowspan = 2;

                            PdfPCell T5 = new PdfPCell(new Phrase(new Chunk("ชื่อสินค้า", fontNormal)));
                            T5.HorizontalAlignment = 1;
                            T5.Rowspan = 2;

                            PdfPCell T6 = new PdfPCell(new Phrase(new Chunk("ลอน", fontNormal)));
                            T6.HorizontalAlignment = 1;
                            T6.Rowspan = 2;

                            PdfPCell T7 = new PdfPCell(new Phrase(new Chunk("กำหนดส่ง	", fontNormal)));
                            T7.HorizontalAlignment = 1;
                            T7.Colspan = 2;

                            PdfPCell T8 = new PdfPCell(new Phrase(new Chunk("จำนวนสั่ง	", fontNormal)));
                            T8.HorizontalAlignment = 1;
                            T8.Colspan = 2;

                            PdfPCell T9 = new PdfPCell(new Phrase(new Chunk("ยกเลิก", fontNormal)));
                            T9.HorizontalAlignment = 1;
                            T9.Rowspan = 2;

                            PdfPCell T10 = new PdfPCell(new Phrase(new Chunk("Hold", fontNormal)));
                            T10.HorizontalAlignment = 1;
                            T10.Rowspan = 2;

                            PdfPCell T11 = new PdfPCell(new Phrase(new Chunk("สถานะกล่อง", fontNormal)));
                            T11.HorizontalAlignment = 1;
                            T11.Rowspan = 2;

                            PdfPCell T12 = new PdfPCell(new Phrase(new Chunk("สถานะ\nส่วนประกอบ", fontNormal)));
                            T12.HorizontalAlignment = 1;
                            T12.Rowspan = 2;

                            PdfPCell T13 = new PdfPCell(new Phrase(new Chunk("Process", fontNormal)));
                            T13.HorizontalAlignment = 1;
                            T13.Rowspan = 2;

                            PdfPCell T14 = new PdfPCell(new Phrase(new Chunk("หมายเหตุ", fontNormal)));
                            T14.HorizontalAlignment = 1;
                            T14.Rowspan = 2;

                            tableData.AddCell(T1);
                            tableData.AddCell(T2);
                            tableData.AddCell(T3);
                            tableData.AddCell(T4);
                            tableData.AddCell(T5);
                            tableData.AddCell(T6);
                            tableData.AddCell(T7);
                            tableData.AddCell(T8);
                            tableData.AddCell(T9);
                            tableData.AddCell(T10);
                            tableData.AddCell(T11);
                            tableData.AddCell(T12);
                            tableData.AddCell(T13);
                            tableData.AddCell(T14);

                            PdfPCell T15 = new PdfPCell(new Phrase(new Chunk("เดิม", fontNormal)));
                            T15.HorizontalAlignment = 1;
                            //T15.Rowspan = 2;

                            PdfPCell T16 = new PdfPCell(new Phrase(new Chunk("ใหม่", fontNormal)));
                            T16.HorizontalAlignment = 1;
                            //T16.Rowspan = 2;

                            PdfPCell T17 = new PdfPCell(new Phrase(new Chunk("เดิม", fontNormal)));
                            T17.HorizontalAlignment = 1;
                            //T17.Rowspan = 2;

                            PdfPCell T18 = new PdfPCell(new Phrase(new Chunk("ใหม่", fontNormal)));
                            T18.HorizontalAlignment = 1;
                            //T18.Rowspan = 2;

                            tableData.AddCell(T15);
                            tableData.AddCell(T16);
                            tableData.AddCell(T17);
                            tableData.AddCell(T18);

                            for (int i = 1; i <= 8; i++)
                            {
                                allCounterLoop = allCounterLoop + 1;
                                if (allCounterLoop <= data.reportDocumentSlists.Count)
                                {
                                    //data.reportDocumentSlists[i].BoxStatus;
                                    PdfPCell C1 = new PdfPCell(new Phrase(new Chunk(allCounterLoop.ToString(), fontNormal)));
                                    C1.HorizontalAlignment = 1;

                                    PdfPCell C2 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].OrderItem.Trim(), fontNormal)));
                                    C2.HorizontalAlignment = 1;

                                    PdfPCell C3 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].Pc.Trim(), fontNormal)));
                                    C3.HorizontalAlignment = 1;

                                    PdfPCell C4 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].MaterialNo.Trim(), fontNormal)));
                                    C4.HorizontalAlignment = 1;

                                    PdfPCell C5 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].MatDesc, fontNormal)));
                                    C5.HorizontalAlignment = 0;

                                    PdfPCell C6 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].Flute.Trim(), fontNormal)));
                                    C6.HorizontalAlignment = 1;

                                    string dueOld = "";
                                    try { dueOld = data.reportDocumentSlists[allCounterLoop - 1].DuedateOld.Value.ToString("dd/MM/yyyy"); } catch { dueOld = ""; }
                                    PdfPCell C7 = new PdfPCell(new Phrase(new Chunk(dueOld, fontNormal)));
                                    C7.HorizontalAlignment = 1;

                                    string dueNew = "";
                                    try { dueNew = data.reportDocumentSlists[allCounterLoop - 1].DuedateNew.Value.ToString("dd/MM/yyyy") == data.reportDocumentSlists[allCounterLoop - 1].DuedateOld.Value.ToString("dd/MM/yyyy") ? "" : data.reportDocumentSlists[allCounterLoop - 1].DuedateNew.Value.ToString("dd/MM/yyyy"); } catch { dueNew = ""; }
                                    PdfPCell C8 = new PdfPCell(new Phrase(new Chunk(dueNew, fontNormal)));
                                    C8.HorizontalAlignment = 1;

                                    PdfPCell C9 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].OrderQtyOld == null ? "" : string.Format("{0:N0}", data.reportDocumentSlists[allCounterLoop - 1].OrderQtyOld), fontNormal)));
                                    C9.HorizontalAlignment = 1;

                                    string olddata = string.Empty;
                                    try
                                    {
                                        olddata = data.reportDocumentSlists[allCounterLoop - 1].OrderQtyNew == data.reportDocumentSlists[allCounterLoop - 1].OrderQtyOld ? null : string.Format("{0:N0}", data.reportDocumentSlists[allCounterLoop - 1].OrderQtyNew);
                                    }
                                    catch
                                    {
                                        olddata = "";
                                    }

                                    PdfPCell C10 = new PdfPCell(new Phrase(new Chunk(olddata, fontNormal)));
                                    C10.HorizontalAlignment = 1;


                                    PdfPCell C11 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].Cancel == true ? "/" : "", fontNormal)));
                                    C11.HorizontalAlignment = 1;

                                    PdfPCell C12 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].Hold == true ? "/" : "", fontNormal)));
                                    C12.HorizontalAlignment = 1;

                                    PdfPCell C13 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].BoxStatus, fontNormal)));
                                    C13.HorizontalAlignment = 0;

                                    PdfPCell C14 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].PartStatus, fontNormal)));
                                    C14.HorizontalAlignment = 0;

                                    PdfPCell C15 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].Process, fontNormal)));
                                    C15.HorizontalAlignment = 0;

                                    PdfPCell C16 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].Remark, fontNormal)));
                                    C16.HorizontalAlignment = 0;

                                    tableData.AddCell(C1);
                                    tableData.AddCell(C2);
                                    tableData.AddCell(C3);
                                    tableData.AddCell(C4);
                                    tableData.AddCell(C5);
                                    tableData.AddCell(C6);
                                    tableData.AddCell(C7);
                                    tableData.AddCell(C8);
                                    tableData.AddCell(C9);
                                    tableData.AddCell(C10);
                                    tableData.AddCell(C11);
                                    tableData.AddCell(C12);
                                    tableData.AddCell(C13);
                                    tableData.AddCell(C14);
                                    tableData.AddCell(C15);
                                    tableData.AddCell(C16);

                                }
                                else
                                {
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                }
                            }

                            statusNextPage = true;
                            document.Add(tableData);
                            #endregion

                            #region [Footer]

                            document.Add(new Paragraph(new Chunk("          ", fontNormal)));
                            float[] columnWidths3 = { 3, 3, 3, 3 };
                            PdfPTable tabledFooter = new PdfPTable(columnWidths3);
                            tabledFooter.WidthPercentage = 100f;
                            tabledFooter.DefaultCell.Border = 0;

                            Phrase pCreate = new Phrase();
                            string createuser = string.Empty;
                            if (string.IsNullOrEmpty(data.UserCreate))
                            {
                                createuser = "ผู้เสนอ ____________________________บริการลูกค้า";
                                var c1 = new Chunk(createuser, fontNormal);
                                pCreate.Add(c1);
                            }
                            else
                            {
                                //createuser = "ผู้เสนอ " + data.UserCreate + " บริการลูกค้า";
                                var c0 = new Chunk("ผู้เสนอ ", fontNormal);
                                var c1 = new Chunk(data.UserCreate, fontUnderLine);
                                var c2 = new Chunk(" บริการลูกค้า", fontNormal);
                                pCreate.Add(c0);
                                pCreate.Add(c1);
                                pCreate.Add(c2);
                                //var c1 = new Chunk(createuser, fontNormal);
                                //pCreate.Add(c1);
                            }

                            PdfPCell F1 = new PdfPCell(pCreate);
                            F1.HorizontalAlignment = 0;
                            F1.BorderColor = BaseColor.White;
                            F1.Border = 0;
                            PdfPCell F2 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F2.HorizontalAlignment = 0;
                            F2.BorderColor = BaseColor.White;
                            F2.Border = 0;
                            PdfPCell F3 = new PdfPCell(new Phrase(new Chunk("ผ.วางแผน ...............................................", fontNormal)));
                            F3.HorizontalAlignment = 0;
                            F3.BorderColor = BaseColor.White;
                            F3.Border = 0;
                            PdfPCell F4 = new PdfPCell(new Phrase(new Chunk("หน่วยงานพิมพ์ .............................................", fontNormal)));
                            F4.HorizontalAlignment = 0;
                            F4.BorderColor = BaseColor.White;
                            F4.Border = 0;

                            tabledFooter.AddCell(F1);
                            tabledFooter.AddCell(F2);
                            tabledFooter.AddCell(F3);
                            tabledFooter.AddCell(F4);

                            PdfPCell F11 = new PdfPCell(new Phrase(new Chunk("วันที่ " + DateTime.Now.ToString("dd/MM/yyyy"), fontNormal)));
                            F11.HorizontalAlignment = 0;
                            F11.BorderColor = BaseColor.White;
                            F11.Border = 0;
                            PdfPCell F12 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F12.HorizontalAlignment = 0;
                            F12.BorderColor = BaseColor.White;
                            F12.Border = 0;
                            PdfPCell F13 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                            F13.HorizontalAlignment = 0;
                            F13.BorderColor = BaseColor.White;
                            F13.Border = 0;
                            PdfPCell F14 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                            F14.HorizontalAlignment = 0;
                            F14.BorderColor = BaseColor.White;
                            F14.Border = 0;

                            tabledFooter.AddCell(F11);
                            tabledFooter.AddCell(F12);
                            tabledFooter.AddCell(F13);
                            tabledFooter.AddCell(F14);

                            tabledFooter.AddCell(F12);
                            tabledFooter.AddCell(F12);
                            tabledFooter.AddCell(F12);
                            tabledFooter.AddCell(F12);

                            PdfPCell F21 = new PdfPCell(new Phrase(new Chunk("หน่วยงานสำเร็จรูป .............................................", fontNormal)));
                            F21.HorizontalAlignment = 0;
                            F21.BorderColor = BaseColor.White;
                            F21.Border = 0;
                            PdfPCell F22 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F22.HorizontalAlignment = 0;
                            F22.BorderColor = BaseColor.White;
                            F22.Border = 0;
                            PdfPCell F23 = new PdfPCell(new Phrase(new Chunk("ผ.ผลิตแผ่นลูกฟูก .............................................", fontNormal)));
                            F23.HorizontalAlignment = 0;
                            F23.BorderColor = BaseColor.White;
                            F23.Border = 0;
                            PdfPCell F24 = new PdfPCell(new Phrase(new Chunk("ผ.คลังสินค้า/ขนส่ง .............................................", fontNormal)));
                            F24.HorizontalAlignment = 0;
                            F24.BorderColor = BaseColor.White;
                            F24.Border = 0;

                            tabledFooter.AddCell(F21);
                            tabledFooter.AddCell(F22);
                            tabledFooter.AddCell(F23);
                            tabledFooter.AddCell(F24);

                            PdfPCell F31 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                            F31.HorizontalAlignment = 0;
                            F31.BorderColor = BaseColor.White;
                            F31.Border = 0;
                            PdfPCell F32 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F32.HorizontalAlignment = 0;
                            F32.BorderColor = BaseColor.White;
                            F32.Border = 0;
                            PdfPCell F33 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                            F33.HorizontalAlignment = 0;
                            F33.BorderColor = BaseColor.White;
                            F33.Border = 0;
                            PdfPCell F34 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                            F34.HorizontalAlignment = 0;
                            F34.BorderColor = BaseColor.White;
                            F34.Border = 0;

                            tabledFooter.AddCell(F31);
                            tabledFooter.AddCell(F32);
                            tabledFooter.AddCell(F33);
                            tabledFooter.AddCell(F34);

                            tabledFooter.AddCell(F12);
                            tabledFooter.AddCell(F12);
                            tabledFooter.AddCell(F12);
                            tabledFooter.AddCell(F12);

                            PdfPCell F41 = new PdfPCell(new Phrase(new Chunk("รหัสเอกสาร " + data.SDocName, fontNormal)));
                            F41.HorizontalAlignment = 0;
                            F41.BorderColor = BaseColor.White;
                            F41.Border = 0;
                            PdfPCell F42 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F42.HorizontalAlignment = 0;
                            F42.BorderColor = BaseColor.White;
                            F42.Border = 0;
                            PdfPCell F43 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F43.HorizontalAlignment = 0;
                            F43.BorderColor = BaseColor.White;
                            F43.Border = 0;
                            PdfPCell F44 = new PdfPCell(new Phrase(new Chunk("แก้ไขครั้งที่  00", fontNormal)));
                            F44.HorizontalAlignment = 0;
                            F44.BorderColor = BaseColor.White;
                            F44.Border = 0;

                            tabledFooter.AddCell(F41);
                            tabledFooter.AddCell(F42);
                            tabledFooter.AddCell(F43);
                            tabledFooter.AddCell(F44);

                            PdfPCell F51 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F51.HorizontalAlignment = 0;
                            F51.BorderColor = BaseColor.White;
                            F51.Border = 0;
                            PdfPCell F52 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F52.HorizontalAlignment = 0;
                            F52.BorderColor = BaseColor.White;
                            F52.Border = 0;
                            PdfPCell F53 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F53.HorizontalAlignment = 0;
                            F53.BorderColor = BaseColor.White;
                            F53.Border = 0;
                            PdfPCell F54 = new PdfPCell(new Phrase(new Chunk("วันที่เริ่มใช้ " + data.SDocDate, fontNormal)));
                            F54.HorizontalAlignment = 0;
                            F54.BorderColor = BaseColor.White;
                            F54.Border = 0;

                            tabledFooter.AddCell(F51);
                            tabledFooter.AddCell(F52);
                            tabledFooter.AddCell(F53);
                            tabledFooter.AddCell(F54);

                            document.Add(tabledFooter);



                            #endregion

                        }


                    }
                }
                else
                {
                    int indexloop = 0;
                    int caseDiffTrans = 0;
                    int totalTbl = data.reportDocumentSlists.Count;
                    decimal calpage = (decimal)(totalTbl - caseDiffTrans) / 10;
                    int pageNextTotal = (int)Math.Ceiling(calpage);
                    bool statusNextPage = false;
                    int allCounterLoop = 0;
                    if (pageNextTotal == 0)
                    {
                        #region [Header]

                        document.Add(new Paragraph(new Chunk("          ", fontNormal)));
                        float[] columnWidths1 = { 3, 1, 5, 3 };
                        PdfPTable tableHeader = new PdfPTable(columnWidths1);
                        tableHeader.WidthPercentage = 100f;


                        PdfPCell h1 = new PdfPCell(new Phrase(new Chunk("เอกสารควบคุม", fontUnderLine)));
                        h1.HorizontalAlignment = 0;
                        //h1.Rowspan = 2;
                        h1.BorderColor = BaseColor.White;
                        h1.Border = 0;


                        var cimage = new Chunk(image, 0, 0, false);
                        var h2 = new PdfPCell { PaddingLeft = 0, PaddingTop = 0, PaddingBottom = 0, PaddingRight = 2 };
                        h2.AddElement(cimage);
                        h2.HorizontalAlignment = Element.ALIGN_LEFT;
                        h2.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //h2.FixedHeight = 20;

                        h2.BorderColor = BaseColor.White;
                        h2.Border = 0;

                        //PdfPCell h2 = new PdfPCell(new Phrase(new Chunk(image, 0, 0)));
                        //h2.HorizontalAlignment = 2;
                        //h2.Rowspan = 2;
                        //h2.BorderColor = BaseColor.White;
                        //h2.Border = 0;
                        var h2t = new Chunk("           ใบแจ้งเปลี่ยนแปลง S/O", fontHeader);
                        var h2text = new Phrase(new Chunk(data.CompanyTh + "\n" + data.CompanyEn, fontHeader2));
                        var h3 = new PdfPCell { PaddingLeft = 50, PaddingTop = 10, PaddingBottom = 0, PaddingRight = 2 };
                        h3.AddElement(h2text);
                        h3.AddElement(h2t);
                        //h3.HorizontalAlignment = 0;
                        //h3.VerticalAlignment = 0;

                        //h3.Rowspan = 2;
                        h3.BorderColor = BaseColor.White;
                        h3.Border = 0;

                        PdfPCell h4 = new PdfPCell(new Phrase(new Chunk("เลขที่ " + data.Docsnumber, fontNormal)));
                        h4.HorizontalAlignment = 2;
                        //h4.Rowspan = 2;
                        h4.BorderColor = BaseColor.White;
                        h4.Border = 0;

                        tableHeader.AddCell(h1);
                        tableHeader.AddCell(h2);
                        tableHeader.AddCell(h3);
                        tableHeader.AddCell(h4);

                        document.Add(tableHeader);


                        //Paragraph pHeader1 = new Paragraph(new Chunk("ใบแจ้งเปลี่ยนแปลง S/O", fontHeader));
                        //pHeader1.Alignment = Element.ALIGN_CENTER;
                        //Paragraph pHeader2 = new Paragraph(new Chunk("ลูกค้า" + data.reportDocumentSlists != null ? data.reportDocumentSlists.Select(x => x.CustomerName).FirstOrDefault() : "", fontHeader2));

                        Phrase pCustomer = new Phrase();
                        string customer = string.Empty;
                        if (data.reportDocumentSlists.Count == 0)
                        {
                            customer = "ลูกค้า ________________________________________________________";
                            var c1 = new Chunk(customer, fontNormal);
                            pCustomer.Add(c1);
                        }
                        else
                        {
                            //createuser = "ผู้เสนอ " + data.UserCreate + " บริการลูกค้า";
                            var c0 = new Chunk("ลูกค้า ", fontNormal);
                            var c1 = new Chunk(data.reportDocumentSlists.Select(x => x.CustomerName).FirstOrDefault(), fontUnderLine);

                            pCustomer.Add(c0);
                            pCustomer.Add(c1);

                            //var c1 = new Chunk(createuser, fontNormal);
                            //pCreate.Add(c1);
                        }

                        Paragraph pHeader2 = new Paragraph(pCustomer);
                        pHeader2.Alignment = Element.ALIGN_LEFT;

                        //document.Add(pHeader1);
                        document.Add(pHeader2);

                        #endregion

                        #region [Table Data]
                        document.Add(new Paragraph(new Chunk("          ", fontNormal)));
                        float[] columnWidths2 = { 1, 3, 3, 3, 2, 2, 2, 2, 2, 2, 2, 3, 3, 4, 4 };
                        PdfPTable tableData = new PdfPTable(columnWidths2);
                        tableData.WidthPercentage = 100f;

                        PdfPCell T1 = new PdfPCell(new Phrase(new Chunk("No", fontNormal)));
                        T1.HorizontalAlignment = 1;
                        T1.Rowspan = 2;

                        PdfPCell T2 = new PdfPCell(new Phrase(new Chunk("S/O", fontNormal)));
                        T2.HorizontalAlignment = 1;
                        T2.Rowspan = 2;

                        PdfPCell T3 = new PdfPCell(new Phrase(new Chunk("P/C", fontNormal)));
                        T3.HorizontalAlignment = 1;
                        T3.Rowspan = 2;

                        PdfPCell T4 = new PdfPCell(new Phrase(new Chunk("Material No", fontNormal)));
                        T4.HorizontalAlignment = 1;
                        T4.Rowspan = 2;

                        //PdfPCell T5 = new PdfPCell(new Phrase(new Chunk("ชื่อสินค้า", fontNormal)));
                        //T5.HorizontalAlignment = 1;
                        //T5.Rowspan = 2;

                        PdfPCell T6 = new PdfPCell(new Phrase(new Chunk("ลอน", fontNormal)));
                        T6.HorizontalAlignment = 1;
                        T6.Rowspan = 2;

                        PdfPCell T7 = new PdfPCell(new Phrase(new Chunk("กำหนดส่ง	", fontNormal)));
                        T7.HorizontalAlignment = 1;
                        T7.Colspan = 2;

                        PdfPCell T8 = new PdfPCell(new Phrase(new Chunk("จำนวนสั่ง	", fontNormal)));
                        T8.HorizontalAlignment = 1;
                        T8.Colspan = 2;

                        PdfPCell T9 = new PdfPCell(new Phrase(new Chunk("ยกเลิก", fontNormal)));
                        T9.HorizontalAlignment = 1;
                        T9.Rowspan = 2;

                        PdfPCell T10 = new PdfPCell(new Phrase(new Chunk("Hold", fontNormal)));
                        T10.HorizontalAlignment = 1;
                        T10.Rowspan = 2;

                        PdfPCell T11 = new PdfPCell(new Phrase(new Chunk("สถานะกล่อง", fontNormal)));
                        T11.HorizontalAlignment = 1;
                        T11.Rowspan = 2;

                        PdfPCell T12 = new PdfPCell(new Phrase(new Chunk("สถานะ\nส่วนประกอบ", fontNormal)));
                        T12.HorizontalAlignment = 1;
                        T12.Rowspan = 2;

                        PdfPCell T13 = new PdfPCell(new Phrase(new Chunk("Process", fontNormal)));
                        T13.HorizontalAlignment = 1;
                        T13.Rowspan = 2;

                        PdfPCell T14 = new PdfPCell(new Phrase(new Chunk("หมายเหตุ", fontNormal)));
                        T14.HorizontalAlignment = 1;
                        T14.Rowspan = 2;

                        tableData.AddCell(T1);
                        tableData.AddCell(T2);
                        tableData.AddCell(T3);
                        tableData.AddCell(T4);
                        //tableData.AddCell(T5);
                        tableData.AddCell(T6);
                        tableData.AddCell(T7);
                        tableData.AddCell(T8);
                        tableData.AddCell(T9);
                        tableData.AddCell(T10);
                        tableData.AddCell(T11);
                        tableData.AddCell(T12);
                        tableData.AddCell(T13);
                        tableData.AddCell(T14);

                        PdfPCell T15 = new PdfPCell(new Phrase(new Chunk("เดิม", fontNormal)));
                        T15.HorizontalAlignment = 1;
                        //T15.Rowspan = 2;

                        PdfPCell T16 = new PdfPCell(new Phrase(new Chunk("ใหม่", fontNormal)));
                        T16.HorizontalAlignment = 1;
                        //T16.Rowspan = 2;

                        PdfPCell T17 = new PdfPCell(new Phrase(new Chunk("เดิม", fontNormal)));
                        T17.HorizontalAlignment = 1;
                        //T17.Rowspan = 2;

                        PdfPCell T18 = new PdfPCell(new Phrase(new Chunk("ใหม่", fontNormal)));
                        T18.HorizontalAlignment = 1;
                        //T18.Rowspan = 2;

                        tableData.AddCell(T15);
                        tableData.AddCell(T16);
                        tableData.AddCell(T17);
                        tableData.AddCell(T18);

                        for (int i = 1; i <= 5; i++)
                        {
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            //tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                            tableData.AddCell("  ");
                        }

                        document.Add(tableData);
                        #endregion

                        #region [Footer]

                        document.Add(new Paragraph(new Chunk("          ", fontNormal)));
                        float[] columnWidths3 = { 3, 3, 3, 3 };
                        PdfPTable tabledFooter = new PdfPTable(columnWidths3);
                        tabledFooter.WidthPercentage = 100f;
                        tabledFooter.DefaultCell.Border = 0;

                        Phrase pCreate = new Phrase();
                        string createuser = string.Empty;
                        if (string.IsNullOrEmpty(data.UserCreate))
                        {
                            createuser = "ผู้เสนอ ____________________________บริการลูกค้า";
                            var c1 = new Chunk(createuser, fontNormal);
                            pCreate.Add(c1);
                        }
                        else
                        {
                            //createuser = "ผู้เสนอ " + data.UserCreate + " บริการลูกค้า";
                            var c0 = new Chunk("ผู้เสนอ ", fontNormal);
                            var c1 = new Chunk(data.UserCreate, fontUnderLine);
                            var c2 = new Chunk(" บริการลูกค้า", fontNormal);
                            pCreate.Add(c0);
                            pCreate.Add(c1);
                            pCreate.Add(c2);
                            //var c1 = new Chunk(createuser, fontNormal);
                            //pCreate.Add(c1);
                        }

                        PdfPCell F1 = new PdfPCell(pCreate);
                        F1.HorizontalAlignment = 0;
                        F1.BorderColor = BaseColor.White;
                        F1.Border = 0;
                        PdfPCell F2 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F2.HorizontalAlignment = 0;
                        F2.BorderColor = BaseColor.White;
                        F2.Border = 0;
                        PdfPCell F3 = new PdfPCell(new Phrase(new Chunk("ผู้พิจารณา ...............................................", fontNormal)));
                        F3.HorizontalAlignment = 0;
                        F3.BorderColor = BaseColor.White;
                        F3.Border = 0;
                        PdfPCell F4 = new PdfPCell(new Phrase(new Chunk("ผู้อนุมัติ .............................................", fontNormal)));
                        F4.HorizontalAlignment = 0;
                        F4.BorderColor = BaseColor.White;
                        F4.Border = 0;

                        tabledFooter.AddCell(F1);
                        tabledFooter.AddCell(F2);
                        tabledFooter.AddCell(F3);
                        tabledFooter.AddCell(F4);

                        PdfPCell F11 = new PdfPCell(new Phrase(new Chunk("วันที่ " + DateTime.Now.ToString("dd/MM/yyyy"), fontNormal)));
                        F11.HorizontalAlignment = 0;
                        F11.BorderColor = BaseColor.White;
                        F11.Border = 0;
                        PdfPCell F12 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F12.HorizontalAlignment = 0;
                        F12.BorderColor = BaseColor.White;
                        F12.Border = 0;
                        PdfPCell F13 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                        F13.HorizontalAlignment = 0;
                        F13.BorderColor = BaseColor.White;
                        F13.Border = 0;
                        PdfPCell F14 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                        F14.HorizontalAlignment = 0;
                        F14.BorderColor = BaseColor.White;
                        F14.Border = 0;

                        tabledFooter.AddCell(F11);
                        tabledFooter.AddCell(F12);
                        tabledFooter.AddCell(F13);
                        tabledFooter.AddCell(F14);

                        tabledFooter.AddCell(F12);
                        tabledFooter.AddCell(F12);
                        tabledFooter.AddCell(F12);
                        tabledFooter.AddCell(F12);

                        PdfPCell F41 = new PdfPCell(new Phrase(new Chunk("รหัสเอกสาร " + data.SDocName, fontNormal)));
                        F41.HorizontalAlignment = 0;
                        F41.BorderColor = BaseColor.White;
                        F41.Border = 0;
                        PdfPCell F42 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F42.HorizontalAlignment = 0;
                        F42.BorderColor = BaseColor.White;
                        F42.Border = 0;
                        PdfPCell F43 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F43.HorizontalAlignment = 0;
                        F43.BorderColor = BaseColor.White;
                        F43.Border = 0;
                        PdfPCell F44 = new PdfPCell(new Phrase(new Chunk("แก้ไขครั้งที่", fontNormal)));
                        F44.HorizontalAlignment = 0;
                        F44.BorderColor = BaseColor.White;
                        F44.Border = 0;

                        tabledFooter.AddCell(F41);
                        tabledFooter.AddCell(F42);
                        tabledFooter.AddCell(F43);
                        tabledFooter.AddCell(F44);

                        PdfPCell F51 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F51.HorizontalAlignment = 0;
                        F51.BorderColor = BaseColor.White;
                        F51.Border = 0;
                        PdfPCell F52 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F52.HorizontalAlignment = 0;
                        F52.BorderColor = BaseColor.White;
                        F52.Border = 0;
                        PdfPCell F53 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                        F53.HorizontalAlignment = 0;
                        F53.BorderColor = BaseColor.White;
                        F53.Border = 0;
                        PdfPCell F54 = new PdfPCell(new Phrase(new Chunk("วันที่เริ่มใช้ " + data.SDocDate, fontNormal)));
                        F54.HorizontalAlignment = 0;
                        F54.BorderColor = BaseColor.White;
                        F54.Border = 0;

                        tabledFooter.AddCell(F51);
                        tabledFooter.AddCell(F52);
                        tabledFooter.AddCell(F53);
                        tabledFooter.AddCell(F54);

                        document.Add(tabledFooter);



                        #endregion

                    }
                    else
                    {

                        for (int j = 1; j <= pageNextTotal; j++)
                        {
                            if (statusNextPage)
                            {
                                document.SetPageSize(PageSize.A4.Rotate());
                                document.SetMargins(20f, 20f, 5f, 5f);
                                document.NewPage();
                            }

                            #region [Header]

                            document.Add(new Paragraph(new Chunk("          ", fontNormal)));
                            float[] columnWidths1 = { 3, 1, 5, 3 };
                            PdfPTable tableHeader = new PdfPTable(columnWidths1);
                            tableHeader.WidthPercentage = 100f;


                            PdfPCell h1 = new PdfPCell(new Phrase(new Chunk("เอกสารควบคุม", fontUnderLine)));
                            h1.HorizontalAlignment = 0;
                            //h1.Rowspan = 2;
                            h1.BorderColor = BaseColor.White;
                            h1.Border = 0;


                            var cimage = new Chunk(image, 0, 0, false);
                            var h2 = new PdfPCell { PaddingLeft = 0, PaddingTop = 0, PaddingBottom = 0, PaddingRight = 2 };
                            h2.AddElement(cimage);
                            h2.HorizontalAlignment = Element.ALIGN_LEFT;
                            h2.VerticalAlignment = Element.ALIGN_MIDDLE;
                            //h2.FixedHeight = 20;

                            h2.BorderColor = BaseColor.White;
                            h2.Border = 0;

                            //PdfPCell h2 = new PdfPCell(new Phrase(new Chunk(image, 0, 0)));
                            //h2.HorizontalAlignment = 2;
                            //h2.Rowspan = 2;
                            //h2.BorderColor = BaseColor.White;
                            //h2.Border = 0;
                            var h2t = new Chunk("           ใบแจ้งเปลี่ยนแปลง S/O", fontHeader);
                            var h2text = new Phrase(new Chunk(data.CompanyTh + "\n" + data.CompanyEn, fontHeader2));
                            var h3 = new PdfPCell { PaddingLeft = 50, PaddingTop = 10, PaddingBottom = 0, PaddingRight = 2 };
                            h3.AddElement(h2text);
                            h3.AddElement(h2t);
                            //h3.HorizontalAlignment = 0;
                            //h3.VerticalAlignment = 0;

                            //h3.Rowspan = 2;
                            h3.BorderColor = BaseColor.White;
                            h3.Border = 0;

                            PdfPCell h4 = new PdfPCell(new Phrase(new Chunk("เลขที่ " + data.Docsnumber, fontNormal)));
                            h4.HorizontalAlignment = 2;
                            //h4.Rowspan = 2;
                            h4.BorderColor = BaseColor.White;
                            h4.Border = 0;

                            tableHeader.AddCell(h1);
                            tableHeader.AddCell(h2);
                            tableHeader.AddCell(h3);
                            tableHeader.AddCell(h4);

                            document.Add(tableHeader);


                            //Paragraph pHeader1 = new Paragraph(new Chunk("ใบแจ้งเปลี่ยนแปลง S/O", fontHeader));
                            //pHeader1.Alignment = Element.ALIGN_CENTER;
                            //Paragraph pHeader2 = new Paragraph(new Chunk("ลูกค้า" + data.reportDocumentSlists != null ? data.reportDocumentSlists.Select(x => x.CustomerName).FirstOrDefault() : "", fontHeader2));

                            Phrase pCustomer = new Phrase();
                            string customer = string.Empty;
                            if (data.reportDocumentSlists.Count == 0)
                            {
                                customer = "ลูกค้า ________________________________________________________";
                                var c1 = new Chunk(customer, fontNormal);
                                pCustomer.Add(c1);
                            }
                            else
                            {
                                //createuser = "ผู้เสนอ " + data.UserCreate + " บริการลูกค้า";
                                var c0 = new Chunk("ลูกค้า ", fontNormal);
                                var c1 = new Chunk(data.reportDocumentSlists.Select(x => x.CustomerName).FirstOrDefault(), fontUnderLine);

                                pCustomer.Add(c0);
                                pCustomer.Add(c1);

                                //var c1 = new Chunk(createuser, fontNormal);
                                //pCreate.Add(c1);
                            }

                            Paragraph pHeader2 = new Paragraph(pCustomer);
                            pHeader2.Alignment = Element.ALIGN_LEFT;

                            //document.Add(pHeader1);
                            document.Add(pHeader2);

                            #endregion

                            #region [Table Data]
                            document.Add(new Paragraph(new Chunk("          ", fontNormal)));
                            float[] columnWidths2 = { 1, 3, 3, 3, 2, 3, 3, 2, 2, 2, 2, 3, 3, 4, 4 };
                            PdfPTable tableData = new PdfPTable(columnWidths2);
                            tableData.WidthPercentage = 100f;

                            PdfPCell T1 = new PdfPCell(new Phrase(new Chunk("No", fontNormal)));
                            T1.HorizontalAlignment = 1;
                            T1.Rowspan = 2;

                            PdfPCell T2 = new PdfPCell(new Phrase(new Chunk("S/O", fontNormal)));
                            T2.HorizontalAlignment = 1;
                            T2.Rowspan = 2;

                            PdfPCell T3 = new PdfPCell(new Phrase(new Chunk("P/C", fontNormal)));
                            T3.HorizontalAlignment = 1;
                            T3.Rowspan = 2;

                            PdfPCell T4 = new PdfPCell(new Phrase(new Chunk("Material No", fontNormal)));
                            T4.HorizontalAlignment = 1;
                            T4.Rowspan = 2;

                            //PdfPCell T5 = new PdfPCell(new Phrase(new Chunk("ชื่อสินค้า", fontNormal)));
                            //T5.HorizontalAlignment = 1;
                            //T5.Rowspan = 2;

                            PdfPCell T6 = new PdfPCell(new Phrase(new Chunk("ลอน", fontNormal)));
                            T6.HorizontalAlignment = 1;
                            T6.Rowspan = 2;

                            PdfPCell T7 = new PdfPCell(new Phrase(new Chunk("กำหนดส่ง	", fontNormal)));
                            T7.HorizontalAlignment = 1;
                            T7.Colspan = 2;

                            PdfPCell T8 = new PdfPCell(new Phrase(new Chunk("จำนวนสั่ง	", fontNormal)));
                            T8.HorizontalAlignment = 1;
                            T8.Colspan = 2;

                            PdfPCell T9 = new PdfPCell(new Phrase(new Chunk("ยกเลิก", fontNormal)));
                            T9.HorizontalAlignment = 1;
                            T9.Rowspan = 2;

                            PdfPCell T10 = new PdfPCell(new Phrase(new Chunk("Hold", fontNormal)));
                            T10.HorizontalAlignment = 1;
                            T10.Rowspan = 2;

                            PdfPCell T11 = new PdfPCell(new Phrase(new Chunk("สถานะกล่อง", fontNormal)));
                            T11.HorizontalAlignment = 1;
                            T11.Rowspan = 2;

                            PdfPCell T12 = new PdfPCell(new Phrase(new Chunk("สถานะ\nส่วนประกอบ", fontNormal)));
                            T12.HorizontalAlignment = 1;
                            T12.Rowspan = 2;

                            PdfPCell T13 = new PdfPCell(new Phrase(new Chunk("Process", fontNormal)));
                            T13.HorizontalAlignment = 1;
                            T13.Rowspan = 2;

                            PdfPCell T14 = new PdfPCell(new Phrase(new Chunk("หมายเหตุ", fontNormal)));
                            T14.HorizontalAlignment = 1;
                            T14.Rowspan = 2;

                            tableData.AddCell(T1);
                            tableData.AddCell(T2);
                            tableData.AddCell(T3);
                            tableData.AddCell(T4);
                            //tableData.AddCell(T5);
                            tableData.AddCell(T6);
                            tableData.AddCell(T7);
                            tableData.AddCell(T8);
                            tableData.AddCell(T9);
                            tableData.AddCell(T10);
                            tableData.AddCell(T11);
                            tableData.AddCell(T12);
                            tableData.AddCell(T13);
                            tableData.AddCell(T14);

                            PdfPCell T15 = new PdfPCell(new Phrase(new Chunk("เดิม", fontNormal)));
                            T15.HorizontalAlignment = 1;
                            //T15.Rowspan = 2;

                            PdfPCell T16 = new PdfPCell(new Phrase(new Chunk("ใหม่", fontNormal)));
                            T16.HorizontalAlignment = 1;
                            //T16.Rowspan = 2;

                            PdfPCell T17 = new PdfPCell(new Phrase(new Chunk("เดิม", fontNormal)));
                            T17.HorizontalAlignment = 1;
                            //T17.Rowspan = 2;

                            PdfPCell T18 = new PdfPCell(new Phrase(new Chunk("ใหม่", fontNormal)));
                            T18.HorizontalAlignment = 1;
                            //T18.Rowspan = 2;

                            tableData.AddCell(T15);
                            tableData.AddCell(T16);
                            tableData.AddCell(T17);
                            tableData.AddCell(T18);

                            for (int i = 1; i <= 10; i++)
                            {
                                allCounterLoop = allCounterLoop + 1;
                                if (allCounterLoop <= data.reportDocumentSlists.Count)
                                {
                                    //data.reportDocumentSlists[i].BoxStatus;
                                    PdfPCell C1 = new PdfPCell(new Phrase(new Chunk(allCounterLoop.ToString(), fontNormal)));
                                    C1.HorizontalAlignment = 1;

                                    PdfPCell C2 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].OrderItem.Trim(), fontNormal)));
                                    C2.HorizontalAlignment = 1;

                                    PdfPCell C3 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].Pc.Trim(), fontNormal)));
                                    C3.HorizontalAlignment = 1;

                                    PdfPCell C4 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].MaterialNo.Trim(), fontNormal)));
                                    C4.HorizontalAlignment = 1;

                                    //PdfPCell C5 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].CustomerName, fontNormal)));
                                    //C5.HorizontalAlignment = 0;

                                    PdfPCell C6 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].Flute.Trim(), fontNormal)));
                                    C6.HorizontalAlignment = 1;

                                    string dueOld = "";
                                    try { dueOld = data.reportDocumentSlists[allCounterLoop - 1].DuedateOld.Value.ToString("dd/MM/yyyy"); } catch { dueOld = ""; }
                                    PdfPCell C7 = new PdfPCell(new Phrase(new Chunk(dueOld, fontNormal)));
                                    C7.HorizontalAlignment = 1;

                                    string dueNew = "";
                                    try { dueNew = data.reportDocumentSlists[allCounterLoop - 1].DuedateNew.Value.ToString("dd/MM/yyyy") == data.reportDocumentSlists[allCounterLoop - 1].DuedateOld.Value.ToString("dd/MM/yyyy") ? "" : data.reportDocumentSlists[allCounterLoop - 1].DuedateNew.Value.ToString("dd/MM/yyyy"); } catch { dueNew = ""; }
                                    PdfPCell C8 = new PdfPCell(new Phrase(new Chunk(dueNew, fontNormal)));
                                    C8.HorizontalAlignment = 1;

                                    PdfPCell C9 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].OrderQtyOld == null ? "" : string.Format("{0:N0}", data.reportDocumentSlists[allCounterLoop - 1].OrderQtyOld), fontNormal)));
                                    C9.HorizontalAlignment = 1;

                                    string olddata = string.Empty;
                                    try
                                    {
                                        olddata = data.reportDocumentSlists[allCounterLoop - 1].OrderQtyNew == data.reportDocumentSlists[allCounterLoop - 1].OrderQtyOld ? null : string.Format("{0:N0}", data.reportDocumentSlists[allCounterLoop - 1].OrderQtyNew);
                                    }
                                    catch
                                    {
                                        olddata = "";
                                    }

                                    PdfPCell C10 = new PdfPCell(new Phrase(new Chunk(olddata, fontNormal)));
                                    C10.HorizontalAlignment = 1;

                                    PdfPCell C11 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].Cancel == true ? "/" : "", fontNormal)));
                                    C11.HorizontalAlignment = 1;

                                    PdfPCell C12 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].Hold == true ? "/" : "", fontNormal)));
                                    C12.HorizontalAlignment = 1;

                                    PdfPCell C13 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].BoxStatus, fontNormal)));
                                    C13.HorizontalAlignment = 0;

                                    PdfPCell C14 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].PartStatus, fontNormal)));
                                    C14.HorizontalAlignment = 0;

                                    PdfPCell C15 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].Process, fontNormal)));
                                    C15.HorizontalAlignment = 0;

                                    PdfPCell C16 = new PdfPCell(new Phrase(new Chunk(data.reportDocumentSlists[allCounterLoop - 1].Remark, fontNormal)));
                                    C16.HorizontalAlignment = 0;

                                    tableData.AddCell(C1);
                                    tableData.AddCell(C2);
                                    tableData.AddCell(C3);
                                    tableData.AddCell(C4);
                                    //tableData.AddCell(C5);
                                    tableData.AddCell(C6);
                                    tableData.AddCell(C7);
                                    tableData.AddCell(C8);
                                    tableData.AddCell(C9);
                                    tableData.AddCell(C10);
                                    tableData.AddCell(C11);
                                    tableData.AddCell(C12);
                                    tableData.AddCell(C13);
                                    tableData.AddCell(C14);
                                    tableData.AddCell(C15);
                                    tableData.AddCell(C16);

                                }
                                else
                                {
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    //tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                    tableData.AddCell("  ");
                                }
                            }

                            statusNextPage = true;

                            document.Add(tableData);
                            #endregion

                            #region [Footer]

                            document.Add(new Paragraph(new Chunk("          ", fontNormal)));
                            float[] columnWidths3 = { 3, 3, 3, 3 };
                            PdfPTable tabledFooter = new PdfPTable(columnWidths3);
                            tabledFooter.WidthPercentage = 100f;
                            tabledFooter.DefaultCell.Border = 0;

                            Phrase pCreate = new Phrase();
                            string createuser = string.Empty;
                            if (string.IsNullOrEmpty(data.UserCreate))
                            {
                                createuser = "ผู้เสนอ ____________________________บริการลูกค้า";
                                var c1 = new Chunk(createuser, fontNormal);
                                pCreate.Add(c1);
                            }
                            else
                            {
                                //createuser = "ผู้เสนอ " + data.UserCreate + " บริการลูกค้า";
                                var c0 = new Chunk("ผู้เสนอ ", fontNormal);
                                var c1 = new Chunk(data.UserCreate, fontUnderLine);
                                var c2 = new Chunk(" บริการลูกค้า", fontNormal);
                                pCreate.Add(c0);
                                pCreate.Add(c1);
                                pCreate.Add(c2);
                                //var c1 = new Chunk(createuser, fontNormal);
                                //pCreate.Add(c1);
                            }

                            PdfPCell F1 = new PdfPCell(pCreate);
                            F1.HorizontalAlignment = 0;
                            F1.BorderColor = BaseColor.White;
                            F1.Border = 0;
                            PdfPCell F2 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F2.HorizontalAlignment = 0;
                            F2.BorderColor = BaseColor.White;
                            F2.Border = 0;
                            PdfPCell F3 = new PdfPCell(new Phrase(new Chunk("ผู้พิจารณา ...............................................", fontNormal)));
                            F3.HorizontalAlignment = 0;
                            F3.BorderColor = BaseColor.White;
                            F3.Border = 0;
                            PdfPCell F4 = new PdfPCell(new Phrase(new Chunk("ผู้อนุมัติ .............................................", fontNormal)));
                            F4.HorizontalAlignment = 0;
                            F4.BorderColor = BaseColor.White;
                            F4.Border = 0;

                            tabledFooter.AddCell(F1);
                            tabledFooter.AddCell(F2);
                            tabledFooter.AddCell(F3);
                            tabledFooter.AddCell(F4);

                            PdfPCell F11 = new PdfPCell(new Phrase(new Chunk("วันที่ " + DateTime.Now.ToString("dd/MM/yyyy"), fontNormal)));
                            F11.HorizontalAlignment = 0;
                            F11.BorderColor = BaseColor.White;
                            F11.Border = 0;
                            PdfPCell F12 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F12.HorizontalAlignment = 0;
                            F12.BorderColor = BaseColor.White;
                            F12.Border = 0;
                            PdfPCell F13 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                            F13.HorizontalAlignment = 0;
                            F13.BorderColor = BaseColor.White;
                            F13.Border = 0;
                            PdfPCell F14 = new PdfPCell(new Phrase(new Chunk("วันที่........../.............../...............", fontNormal)));
                            F14.HorizontalAlignment = 0;
                            F14.BorderColor = BaseColor.White;
                            F14.Border = 0;

                            tabledFooter.AddCell(F11);
                            tabledFooter.AddCell(F12);
                            tabledFooter.AddCell(F13);
                            tabledFooter.AddCell(F14);

                            tabledFooter.AddCell(F12);
                            tabledFooter.AddCell(F12);
                            tabledFooter.AddCell(F12);
                            tabledFooter.AddCell(F12);

                            PdfPCell F41 = new PdfPCell(new Phrase(new Chunk("รหัสเอกสาร " + data.SDocName, fontNormal)));
                            F41.HorizontalAlignment = 0;
                            F41.BorderColor = BaseColor.White;
                            F41.Border = 0;
                            PdfPCell F42 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F42.HorizontalAlignment = 0;
                            F42.BorderColor = BaseColor.White;
                            F42.Border = 0;
                            PdfPCell F43 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F43.HorizontalAlignment = 0;
                            F43.BorderColor = BaseColor.White;
                            F43.Border = 0;
                            PdfPCell F44 = new PdfPCell(new Phrase(new Chunk("แก้ไขครั้งที่", fontNormal)));
                            F44.HorizontalAlignment = 0;
                            F44.BorderColor = BaseColor.White;
                            F44.Border = 0;

                            tabledFooter.AddCell(F41);
                            tabledFooter.AddCell(F42);
                            tabledFooter.AddCell(F43);
                            tabledFooter.AddCell(F44);

                            PdfPCell F51 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F51.HorizontalAlignment = 0;
                            F51.BorderColor = BaseColor.White;
                            F51.Border = 0;
                            PdfPCell F52 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F52.HorizontalAlignment = 0;
                            F52.BorderColor = BaseColor.White;
                            F52.Border = 0;
                            PdfPCell F53 = new PdfPCell(new Phrase(new Chunk(" ", fontNormal)));
                            F53.HorizontalAlignment = 0;
                            F53.BorderColor = BaseColor.White;
                            F53.Border = 0;
                            PdfPCell F54 = new PdfPCell(new Phrase(new Chunk("วันที่เริ่มใช้ " + data.SDocDate, fontNormal)));
                            F54.HorizontalAlignment = 0;
                            F54.BorderColor = BaseColor.White;
                            F54.Border = 0;

                            tabledFooter.AddCell(F51);
                            tabledFooter.AddCell(F52);
                            tabledFooter.AddCell(F53);
                            tabledFooter.AddCell(F54);

                            document.Add(tabledFooter);



                            #endregion

                        }


                    }


                }

                document.Close();

                //export
                byte[] result = ms.ToArray();
                string pdfname = $"DocumentS-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.pdf";
                return File(result, "application/pdf", pdfname);
            }
        }

        #region Report DocumentS
        [SessionTimeout]
        public IActionResult ReportDocumentS()
        {
            var documentSlists = new List<DocumentSlist>();

            return View(documentSlists);
        }

        [SessionTimeout]
        public JsonResult SearchReportDocumentS(string customerName, string so, string materialNo, string pc)
        {
            bool isSuccess;
            var exceptionMessage = string.Empty;
            var documentSlists = new List<DocumentSlist>();
            try
            {
                //get documentslist from criteria
                _documentSService.SearchReportDocumentS(ref documentSlists, customerName, so, materialNo, pc);

                isSuccess = true;

            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportDocumentSTable", documentSlists) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportDocumentSTable", documentSlists) });
        }

        #endregion
    }
}