using AutoMapper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
//using PMTs.WebApplication.Services;

using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ModelView.MaintenanceBoard;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.Repository;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class MaintenanceBoardController : Controller
    {
        private readonly IMaintenanceBoardService _maintenanceBoardService;
        private readonly INewProductService _newProductService;
        private readonly IExtensionService _extensionService;
        private readonly IEmailService _emailService;

        public MaintenanceBoardController(IMaintenanceBoardService maintenanceBoardService, IExtensionService extensionService, INewProductService newProductService, IEmailService emailService)
        {
            // Initialize Service
            _maintenanceBoardService = maintenanceBoardService;
            _extensionService = extensionService;
            _newProductService = newProductService;
            _emailService = emailService;
        }

        #region BoardManagement

        [SessionTimeout]
        public IActionResult Index()
        {
            BoardCombindMainTainModel maintenanceBoardModel = new BoardCombindMainTainModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

                maintenanceBoardModel = _maintenanceBoardService.getFristBoardCombind();
                SessionExtentions.SetSession(HttpContext.Session, "MaintainBoardSession", maintenanceBoardModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(maintenanceBoardModel);
        }

        [SessionTimeout]
        public JsonResult BindDataAferSave()
        {
            BoardCombindMainTainModel maintenanceBoardModel = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                maintenanceBoardModel = _maintenanceBoardService.getFristBoardCombind();
                SessionExtentions.SetSession(HttpContext.Session, "MaintainBoardSession", maintenanceBoardModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_BoardTable", maintenanceBoardModel));
        }

        [SessionTimeout]
        public PartialViewResult _ViewModal()
        {
            BoardCombindMainTainModel maintenanceBoardModel = new BoardCombindMainTainModel();
            try
            {
                //  maintenanceBoardModel = _maintenanceBoardService.GetSelectFlute(maintenanceBoardModel);
                //  SessionExtentions.SetSession(HttpContext.Session, "MaintainBoardSession", maintenanceBoardModel);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView(maintenanceBoardModel);
        }


        [SessionTimeout]
        public JsonResult ClearModal()
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = SessionExtentions.GetSession<BoardCombindMainTainModel>(HttpContext.Session, "MaintainBoardSession");
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");

            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_ViewModal", model));
        }

        [SessionTimeout]
        public JsonResult ClearModalAfterSave()
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _maintenanceBoardService.getFristBoardCombind();
                SessionExtentions.SetSession(HttpContext.Session, "MaintainBoardSession", model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_ViewModal", model));
        }

        [SessionTimeout]
        public JsonResult EditData()
        {
            BoardCombindMainTainModel maintenanceBoardModel = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var model = SessionExtentions.GetSession<BoardCombindMainTainModel>(HttpContext.Session, "MaintainBoardSession");
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            return Json(maintenanceBoardModel);
        }

        [SessionTimeout]
        public JsonResult SaveEditData()
        {
            MaintenanceBoardModel maintenanceBoardModel = new MaintenanceBoardModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var model = SessionExtentions.GetSession<BoardCombindMainTainModel>(HttpContext.Session, "MaintainBoardSession");
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            return Json(maintenanceBoardModel);
        }

        [SessionTimeout]
        public PartialViewResult _haveFlute()
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            return PartialView(model);
        }

        [SessionTimeout]
        public PartialViewResult _notHaveFlute()
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            return PartialView(model);
        }

        [SessionTimeout]
        public PartialViewResult _preView()
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            return PartialView(model);
        }

        [SessionTimeout]
        public JsonResult RenderNotHaveFlute()
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            return Json(RenderView.RenderRazorViewToString(this, "_notHaveFlute", model));
        }

        [SessionTimeout]
        public JsonResult RenderNotHaveFluteByFlute(string flute)
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var modelsession = SessionExtentions.GetSession<BoardCombindMainTainModel>(HttpContext.Session, "MaintainBoardSession");
                var fluteselect = modelsession.FluteTR.Where(x => x.FluteCode.Trim().ToLower() == flute.Trim().ToLower()).OrderBy(x => x.Item).ToList();
                int y = 1;
                foreach (var item in fluteselect)
                {
                    string grade = "";
                    BoardSpect tmp = new BoardSpect();
                    tmp.Item = item.Item;
                    tmp.Station = item.Station;
                    tmp.Grade = grade;
                    model.BoardSpect.Add(tmp);
                    y++;
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_notHaveFlute", model));
        }

        [SessionTimeout]
        public JsonResult BindRenderNotHaveFluteByFlute(string code)
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var modelsession = SessionExtentions.GetSession<BoardCombindMainTainModel>(HttpContext.Session, "MaintainBoardSession");
                model._BoardCombind = modelsession.BoardCombind.Where(x => x.Code == code).FirstOrDefault();
                string check = model._BoardCombind.StandardBoard.ToString();
                if (check == "False")
                {
                    BoardCombindMainTainModel tmp = new BoardCombindMainTainModel();
                    tmp = _maintenanceBoardService.getAllBoardspectByCode(model._BoardCombind.Code);
                    model.BoardSpect = tmp.BoardSpect;
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_notHaveFlute", model));
        }

        [SessionTimeout]
        public JsonResult RenderHaveFluteClear()
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            return Json(RenderView.RenderRazorViewToString(this, "_haveFlute", model));
        }

        [SessionTimeout]
        public JsonResult RenderHaveFlute(string boardcombind, string flute)
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var modelsession = SessionExtentions.GetSession<BoardCombindMainTainModel>(HttpContext.Session, "MaintainBoardSession");
                string[] arrboard = boardcombind.Split('/');
                var fluteselect = modelsession.FluteTR.Where(x => x.FluteCode.Trim().ToLower() == flute.Trim().ToLower()).OrderBy(x => x.Item).ToList();
                int y = 1;
                foreach (var item in fluteselect)
                {
                    string grade = "";
                    try { grade = arrboard[y]; } catch { grade = ""; }
                    BoardSpect tmp = new BoardSpect();
                    tmp.Item = item.Item;
                    tmp.Station = item.Station;
                    tmp.Grade = grade;
                    model.BoardSpect.Add(tmp);
                    y++;
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_haveFlute", model));
        }

        [SessionTimeout]
        public JsonResult RenderHaveFluteOnChange(string boardcombind)
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            string[] arrboard = boardcombind.Split('/');
            var boardExist = new BoardCombind();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

                var modelsession = SessionExtentions.GetSession<BoardCombindMainTainModel>(HttpContext.Session, "MaintainBoardSession");

                boardExist = modelsession.BoardCombind.Where(w => w.Board_Combine == boardcombind).FirstOrDefault();

                var fluteselect = modelsession.FluteTR.Where(x => x.FluteCode.Trim().ToLower() == arrboard[0].Trim().ToLower()).OrderBy(x => x.Item).ToList();
                int y = 1;
                foreach (var item in fluteselect)
                {
                    string grade = "";
                    try { grade = arrboard[y]; } catch { grade = ""; }
                    BoardSpect tmp = new BoardSpect();
                    tmp.Item = item.Item;
                    tmp.Station = item.Station;
                    tmp.Grade = grade;
                    model.BoardSpect.Add(tmp);
                    y++;
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { model = RenderView.RenderRazorViewToString(this, "_haveFlute", model), flute = arrboard[0], boardExist = boardExist });
        }

        [SessionTimeout]
        public JsonResult BindingResult(string code)
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var modelsession = SessionExtentions.GetSession<BoardCombindMainTainModel>(HttpContext.Session, "MaintainBoardSession");
                model._BoardCombind = modelsession.BoardCombind.Where(x => x.Code == code).FirstOrDefault();
                string check = model._BoardCombind.StandardBoard.ToString();
                if (check == "False")
                {
                    BoardCombindMainTainModel tmp = new BoardCombindMainTainModel();
                    tmp = _maintenanceBoardService.getAllBoardspectByCode(model._BoardCombind.Code);
                    model.BoardSpect = tmp.BoardSpect;
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(model);
        }

        [SessionTimeout]
        public JsonResult GetMaxcodeBoard()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                BoardCombindMainTainModel model = new BoardCombindMainTainModel();
                model = _maintenanceBoardService.getMaxcodeBoardCombind();
                string lastid = IDGenerator.NextID(model.MaxID);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(lastid);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }

        }
        [SessionTimeout]
        public JsonResult GenerateCode()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string code = _maintenanceBoardService.GenerateBoardCombindCode();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(code);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }

        }

        [SessionTimeout]
        [HttpPost]
        public IActionResult Export([FromBody] ExportDataForSAPRequest request)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var base64 = _maintenanceBoardService.GenerateFileDataForSAP(request);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Ok(base64);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw;
            }

        }


        [SessionTimeout]
        [HttpPost]
        public IActionResult AddData(string req, string arrflute, string arrnoflut)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                BoardCombindMainTainModel model = new BoardCombindMainTainModel();
                model._BoardCombind = JsonConvert.DeserializeObject<BoardCombind>(req);
                if (arrflute.Length > 2)
                {
                    model.BoardSpect = JsonConvert.DeserializeObject<List<BoardSpect>>(arrflute);
                }
                if (arrnoflut.Length > 2)
                {
                    model.BoardSpect = JsonConvert.DeserializeObject<List<BoardSpect>>(arrnoflut);
                }
                var result = _maintenanceBoardService.AddBoardcombind(model);
                if(result == true) _emailService.SendNotifyWhenCreatedBoard(model._BoardCombind.Code, model._BoardCombind.Board);

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return RedirectToAction("Index", "MaintenanceBoard");//  Json("success");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //return Json("Faile");
                throw;
            }

        }

        [SessionTimeout]
        [HttpPost]
        public IActionResult UpdateData(string req, string arrflute, string arrnoflut)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                BoardCombindMainTainModel model = new BoardCombindMainTainModel();
                model._BoardCombind = JsonConvert.DeserializeObject<BoardCombind>(req);
                if (arrflute.Length > 2)
                {
                    model.BoardSpect = JsonConvert.DeserializeObject<List<BoardSpect>>(arrflute);
                }
                if (arrnoflut.Length > 2)
                {
                    model.BoardSpect = JsonConvert.DeserializeObject<List<BoardSpect>>(arrnoflut);
                }
                _maintenanceBoardService.UpdateBoardcombind(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                //  return Json("success");
                return RedirectToAction("Index", "MaintenanceBoard");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //  return Json("Faile");
                throw;
            }


        }

        [SessionTimeout]
        public JsonResult Preview(string code, string flute)
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _maintenanceBoardService.getAllBoardspectByCode(code);
                model = pivot(model.BoardSpect, flute);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_preView", model));
        }

        [SessionTimeout]
        public JsonResult PreviewNew(string arrflute, string arrnoflut, string flute)
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                if (arrflute.Length > 2)
                {
                    model.BoardSpect = JsonConvert.DeserializeObject<List<BoardSpect>>(arrflute);
                }
                if (arrnoflut.Length > 2)
                {
                    model.BoardSpect = JsonConvert.DeserializeObject<List<BoardSpect>>(arrnoflut);
                }
                model = pivot(model.BoardSpect, flute);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_preView", model));
        }

        [SessionTimeout]
        public BoardCombindMainTainModel pivot(List<BoardSpect> pp, string flut)
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model.PreviewTblHeader.lon = "ลอน" + "<Br>" + flut;

                var covertmp = pp.Where(x => x.Station.ToUpper().Trim() == "COVER").FirstOrDefault();
                if (covertmp != null)
                {

                    model.PreviewTblHeader.cover = covertmp.Station + "<Br>" + covertmp.Grade;
                    foreach (var item in pp.Where(x => x.Item == covertmp.Item))
                    {
                        item.Station = "";
                        item.Grade = "";
                    }
                }


                var sd = pp.OrderByDescending(x => x.Item).Select(x => x.Item).FirstOrDefault();
                string[] array = new string[sd];
                int i = 0;
                int zz = 0;
                for (int iz = 0; iz < sd; iz++)
                {

                    array[iz] = "";
                    foreach (var a in pp)
                    {
                        if (zz + 1 == a.Item)
                        {

                            array[iz] = a.Station + "<Br>" + a.Grade;
                            break;
                        }
                        // i++;
                    }
                    zz++;
                    // iz++;

                }

                DataTable dt = new DataTable();
                dt.Columns.Add("1");
                dt.Columns.Add("2");
                dt.Columns.Add("3");
                dt.Columns.Add("4");
                dt.Columns.Add("5");
                int y = 0;
                int z = 0;
                int l = 1;
                DataRow dr = dt.NewRow();
                foreach (string s in array)
                {

                    if (z > 4)
                    {
                        dt.Rows.Add(dr);
                        dr = dt.NewRow();
                        z = 0;
                    }

                    if (z <= 4)
                    {
                        dr[z] = s;
                    }

                    int xx = array.Length - l;
                    if (xx == 0)
                    {
                        dt.Rows.Add(dr);
                    }

                    z++;
                    l++;
                }


                int ck_loop = 0;
                List<Preview> exss = new List<Preview>();
                foreach (DataRow row in dt.Rows)
                {
                    ck_loop++;
                    Preview e = new Preview();
                    e.tbl0 = row[0].ToString();
                    e.tbl1 = row[1].ToString();
                    e.tbl2 = row[2].ToString();
                    e.tbl3 = row[3].ToString();
                    e.tbl4 = row[4].ToString();
                    exss.Add(e);
                }

                if (exss.Count <= 3)
                {
                    int limport = 3 - exss.Count;
                    for (int importex = 0; importex < limport; importex++)
                    {
                        Preview e = new Preview();
                        e.tbl0 = "";
                        e.tbl1 = "";
                        e.tbl2 = "";
                        e.tbl3 = "";
                        e.tbl4 = "";
                        exss.Add(e);
                    }
                }

                if (exss.Count > 2)
                {
                    model.PreviewTblHeader.cover_row = (exss.Count() - 2).ToString();
                }
                model.Preview = exss;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            #region[temp 6 col]
            //var sd = pp.OrderByDescending(x => x.Item).Select(x => x.Item).FirstOrDefault();
            //string[] array = new string[sd+2];
            //int i = 0;
            //int zz = 0;
            //for (int iz = 0; iz < sd+2; iz++)
            //{
            //    if (iz == 0)
            //    {
            //        array[iz] = "";
            //    }
            //    else if (iz == 6)
            //    {
            //        array[iz] = "";
            //    }
            //    else
            //    {
            //        array[iz] = "";
            //        foreach (var a in pp)
            //        {
            //            if (zz + 1 == a.Item)
            //            {
            //                array[iz] = a.Station + "<Br>" + a.Grade;
            //                break;
            //            }
            //            // i++;
            //        }
            //        zz++;
            //        // iz++;
            //    }
            //   // zz++;

            //}

            //DataTable dt = new DataTable();
            //dt.Columns.Add("1");
            //dt.Columns.Add("2");
            //dt.Columns.Add("3");
            //dt.Columns.Add("4");
            //dt.Columns.Add("5");
            //dt.Columns.Add("6");
            //int y = 0;
            //int z = 0;
            //int l = 1;
            //DataRow dr = dt.NewRow();
            //foreach (string s in array)
            //{

            //    if (z > 5)
            //    {
            //        dt.Rows.Add(dr);
            //        dr = dt.NewRow();
            //        z = 0;
            //    }

            //    if (z <= 5)
            //    {
            //        dr[z] = s;
            //    }

            //    int xx = array.Length - l;
            //    if (xx == 0)
            //    {
            //        dt.Rows.Add(dr);
            //    }

            //    z++;
            //    l++;
            //}


            //int ck_loop = 0;
            //List<Preview> exss = new List<Preview>();
            //foreach (DataRow row in dt.Rows)
            //{
            //    ck_loop++;
            //    Preview e = new Preview();
            //    if (ck_loop <= 2)
            //    {
            //        e.tbl0 = "ลอน" + "<Br>" + flut;
            //        e.tbl1 = row[1].ToString();
            //        e.tbl2 = row[2].ToString();
            //        e.tbl3 = row[3].ToString();
            //        e.tbl4 = row[4].ToString();
            //        e.tbl5 = row[5].ToString();
            //    }
            //    else 
            //    {
            //        e.tbl0 = row[0].ToString();
            //        e.tbl1 = row[1].ToString();
            //        e.tbl2 = row[2].ToString();
            //        e.tbl3 = row[3].ToString();
            //        e.tbl4 = row[4].ToString();
            //        e.tbl5 = row[5].ToString();
            //    }

            //    exss.Add(e);
            //}
            #endregion

            return model;
        }

        [SessionTimeout]
        public JsonResult CalculateWeigth(string flut, string arrflute, string arrnoflut)
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            double sumweigth = 0;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                if (arrflute.Length > 2)
                {
                    model.BoardSpect = JsonConvert.DeserializeObject<List<BoardSpect>>(arrflute);
                }
                if (arrnoflut.Length > 2)
                {
                    model.BoardSpect = JsonConvert.DeserializeObject<List<BoardSpect>>(arrnoflut);
                }
                var modelsession = SessionExtentions.GetSession<BoardCombindMainTainModel>(HttpContext.Session, "MaintainBoardSession");
                foreach (var item in model.BoardSpect)
                {
                    double a = 0;
                    double b = 0;
                    var tr = modelsession.FluteTR.Where(x => x.FluteCode.Trim().ToLower() == flut.Trim().ToLower() && x.Station.Trim().ToLower() == item.Station.Trim().ToLower()).Select(x => x.Tr).FirstOrDefault();
                    var papergrad = modelsession.PaperGrade.Where(x => x.Grade.Trim().ToLower() == item.Grade.Trim().ToLower()).Select(x => x.BasicWeight).FirstOrDefault();
                    if (string.IsNullOrEmpty(tr.ToString()))
                    {
                        a = 0;
                    }
                    else
                    {
                        a = Convert.ToDouble(tr);
                    }
                    if (string.IsNullOrEmpty(papergrad.ToString()))
                    {
                        b = 0;
                    }
                    else
                    {
                        b = Convert.ToDouble(papergrad);
                    }
                    double sum = a * b;
                    sumweigth = sumweigth + sum;
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { weigth = sumweigth.ToString() });
        }

        [SessionTimeout]
        public JsonResult FindKiwi(string board)
        {
            string kiwi = "";
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string boards = board.Substring(board.IndexOf("/") + 1, board.Length - board.IndexOf("/") - 1);
                kiwi = _newProductService.GetBoardKIWI(boards);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { Kiwi = kiwi });
        }

        [SessionTimeout]
        public JsonResult CheckDuplicateBoardcode(string board)
        {
            var modelsession = SessionExtentions.GetSession<BoardCombindMainTainModel>(HttpContext.Session, "MaintainBoardSession");
            string checkDup = "0";
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var ck = modelsession.BoardCombind.Where(x => x.Code.Trim().ToLower() == board.Trim().ToLower()).FirstOrDefault();
                if (ck != null)
                {
                    checkDup = "1";
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch
            {
                checkDup = "0";
            }
            return Json(new { dup = checkDup });
        }
        #endregion

        public IActionResult LoadData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Customer data
                BoardCombindMainTainModel model = new BoardCombindMainTainModel();
                model = _maintenanceBoardService.getFristBoardCombind();
                //var customerData = (from tempcustomer in _context.CustomerTB
                //                select tempcustomer);
                var customerData = model.BoardCombind;

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    //customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                    if (sortColumn == "Flute" && sortColumnDirection == "asc")
                    {
                        customerData = customerData.OrderBy(x => x.Flute).ToList();
                    }
                    else if (sortColumn == "Flute" && sortColumnDirection == "desc")
                    {
                        customerData = customerData.OrderByDescending(x => x.Flute).ToList();
                    }
                    else if (sortColumn == "Code" && sortColumnDirection == "asc")
                    {
                        customerData = customerData.OrderBy(x => x.Code).ToList();
                    }
                    else if (sortColumn == "Code" && sortColumnDirection == "desc")
                    {
                        customerData = customerData.OrderByDescending(x => x.Code).ToList();
                    }
                    else if (sortColumn == "Board" && sortColumnDirection == "asc")
                    {
                        customerData = customerData.OrderBy(x => x.Code).ToList();
                    }
                    else if (sortColumn == "Board" && sortColumnDirection == "desc")
                    {
                        customerData = customerData.OrderByDescending(x => x.Code).ToList();
                    }

                }
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.Code.ToLower().Contains(searchValue.ToLower()) || m.Flute.ToLower().Contains(searchValue.ToLower()) || m.Board.ToLower().Contains(searchValue.ToLower()) || (m.Flute.ToLower() + " " + m.Board.ToLower()).Contains(searchValue.ToLower())).ToList();
                }
                customerData = customerData.OrderByDescending(o => o.Id).ToList();
                //total number of rows count 
                recordsTotal = customerData.Count();
                //Paging 
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                
                //Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }



        [SessionTimeout]
        public JsonResult GetCostBoardCombindAcc(string Code)
        {
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _maintenanceBoardService.getBoardCombindAccCode(Code);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { View = RenderView.RenderRazorViewToString(this, "CostView", model), Data = model });
        }

        [SessionTimeout]
        public JsonResult UpdateCostBoardCombindAcc(string req)
        {
            string StatusCode = string.Empty;
            BoardCombindMainTainModel model = new BoardCombindMainTainModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var tmp = JsonConvert.DeserializeObject<List<BoardCombindAccUpdate>>(req);
                _maintenanceBoardService.ManageBoardcombindAcc(tmp);
                StatusCode = "Success";
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                StatusCode = "Faile";
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(StatusCode);
        }

    }

}

