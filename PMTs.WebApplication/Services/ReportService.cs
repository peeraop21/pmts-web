using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.Report;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class ReportService : IReportService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMoDataAPIRepository _moDataAPIRepository;
        private readonly IConfigWordingReportAPIRepository _configWordingReportAPIRepository;
        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;
        private readonly IInterfaceSystemAPIAPIRepository interfaceSystemAPIAPIRepository;
        private readonly IMasterDataAPIRepository masterDataAPIRepository;
        private readonly IExtensionService _extensionService;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _firstName;
        private readonly string _telephone;
        private readonly string _token;

        public ReportService(IHttpContextAccessor httpContextAccessor,
            IMoDataAPIRepository moDataAPIRepository,
            IConfigWordingReportAPIRepository configWordingReportAPIRepository,
            ICompanyProfileAPIRepository companyProfileAPIRepository,
            IExtensionService extensionService,
            IInterfaceSystemAPIAPIRepository interfaceSystemAPIAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _moDataAPIRepository = moDataAPIRepository;
            _configWordingReportAPIRepository = configWordingReportAPIRepository;
            _companyProfileAPIRepository = companyProfileAPIRepository;
            _extensionService = extensionService;
            this.interfaceSystemAPIAPIRepository = interfaceSystemAPIAPIRepository;

            // Initialize User Data From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _firstName = userSessionModel.FirstNameTh;
                _telephone = userSessionModel.Telephone;
                _token = userSessionModel.Token;
            }

            this.masterDataAPIRepository = masterDataAPIRepository;
        }

        public void SearchReportCheckOrderQtyTooMuchByDateAndRepeatCount(ref List<CheckOrderQtyTooMuch> checkOrderQtyTooMuch, string dateFrom, string dateTo, string repeatCount)
        {
            checkOrderQtyTooMuch = JsonConvert.DeserializeObject<List<CheckOrderQtyTooMuch>>(_moDataAPIRepository.ReportCheckOrderQtyTooMuch(_factoryCode, dateFrom, dateTo, repeatCount, _token));
        }

        public void SearchReportRepeatOrderItemsByDateAndRepeatCount(ref List<CheckRepeatOrder> checkRepeatOrders, string dateFrom, string dateTo, string repeatCount)
        {
            checkRepeatOrders = JsonConvert.DeserializeObject<List<CheckRepeatOrder>>(_moDataAPIRepository.ReportCheckRepeatOrder(_factoryCode, dateFrom, dateTo, repeatCount, _token));
        }

        public void SearchReportCheckDiffDueDate(ref List<CheckDiffDueDate> CheckDiffDueDate, int datediff, string dateFrom, string dateTo)
        {
            CheckDiffDueDate = JsonConvert.DeserializeObject<List<CheckDiffDueDate>>(_moDataAPIRepository.ReportCheckDiffDueDate(_factoryCode, datediff, dateFrom, dateTo, _token));
        }

        public void ReportCheckDueDateToolong(ref List<CheckDueDateToolong> checkRepeatOrders, int dayCount)
        {
            checkRepeatOrders = JsonConvert.DeserializeObject<List<CheckDueDateToolong>>(_moDataAPIRepository.ReportCheckDueDateToolong(_factoryCode, dayCount, _token));
        }

        public ReportCheckOrderItem ReportCheckOrederItem(string dateFrom, string dateTo)
        {
            return JsonConvert.DeserializeObject<ReportCheckOrderItem>(_moDataAPIRepository.GetReportCheckOrderItem(_username, _factoryCode, dateFrom, dateTo, _token));
        }

        public void SearchReportCheckMOAndKIWI(IConfiguration configuration, ref List<MoDataPrintMastercard> moDatas, string startDueDate, string endDueDate)
        {
            var companyProfile = new CompanyProfile();
            companyProfile = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(_factoryCode, _token));
            moDatas = JsonConvert.DeserializeObject<List<MoDataPrintMastercard>>(_moDataAPIRepository.GetMoDatasByDueDateRange(_factoryCode, startDueDate, endDueDate, _token));
            var moDatasKIWI = new List<MoDataPrintMastercard>();
            var OrderItems = new List<string>();
            OrderItems = moDatas.GroupBy(x => x.OrderItem).Select(g => g.First().SoKiwi).ToList();

            if (companyProfile != null && !string.IsNullOrEmpty(companyProfile.PlaningServer) && !string.IsNullOrEmpty(companyProfile.PlaningPass))
            {
                var password = _extensionService.DecryptPassKiwi(companyProfile.PlaningPass);
                var connectionString = $"Server={companyProfile.PlaningServer};Database={companyProfile.ShortName.ToLower()}_data;Uid={companyProfile.PlaningUser};Pwd={password};";
                var queryString = @"SELECT job_number as SOKiwi " +
                                    $"From {companyProfile.ShortName.ToLower()}_data.JBSPEC Where job_number IN ('{string.Join("','", OrderItems.ToArray())}') ";

                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    //Execute sql query
                    moDatasKIWI = db.Query<MoDataPrintMastercard>(queryString).OrderBy(m => m.SoKiwi).ToList();

                    db.Close();
                }

                moDatas = moDatas.Where(x => !string.IsNullOrEmpty(x.SoKiwi)).ToList();

                moDatas = moDatas.Except(moDatasKIWI, new MODataKIWIComparer()).OrderBy(m => m.OrderItem).ToList();
            }

            //moDatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDatasByDueDateRange(_factoryCode, startDueDate, endDueDate, _token));
            //var moDatasKIWI = new List<MoData>();
            //var OrderItems = new List<string>();
            //OrderItems = moDatas.GroupBy(x => x.OrderItem).Select(g => g.First().SOKiwi).ToList();

            //var queryString = @"SELECT [SalesOrder] as SOKiwi
            //    FROM [dbo].[LINE_UP]
            //    WHERE (Plant = " + $"'{_factoryCode}') AND (SalesOrder IN ('{string.Join("','", OrderItems.ToArray())}')) AND (SeriesNo = '1') AND (SeqNo = '1') ORDER BY UpdatedDate DESC";

            //using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("KIWIConnect")))
            //{
            //    if (db.State == ConnectionState.Closed)
            //        db.Open();
            //    //Execute sql query
            //    moDatasKIWI = db.Query<MoData>(queryString).OrderBy(m => m.SOKiwi).ToList();

            //    db.Close();
            //}

            //moDatas = moDatas.Where(x => !string.IsNullOrEmpty(x.SOKiwi)).ToList();

            //moDatas = moDatas.Except(moDatasKIWI, new MODataKIWIComparer()).OrderBy(m => m.SOKiwi).ToList();
        }

        public void SearchReportCheckMOAndSAP(IConfiguration configuration, ref ReportCheckMOAndTextfileSAPViewModel reportCheckMOAndTextfileSAPViewModel, string startDueDate, string endDueDate, string configWordingString)
        {
            var moDatasSAP = new List<MoData>();
            var OrderItems = new List<string>();
            var configStr = string.Empty;
            var configArr = !string.IsNullOrEmpty(configWordingString) ? configWordingString.Split(',') : null;
            if (configArr != null)
            {
                foreach (var config in configArr)
                {
                    configStr = configStr + $" and si.[Item Note] not like '%{config}%' ";
                }
            }

            var queryString = @"SELECT Case
	            When ss.[item no] <= 99 Then CONCAT(RTRIM(LTRIM(ss.[sales document no])),RTRIM(LTRIM(REVERSE(ss.[item no]))))
	            ELSE CONCAT(RTRIM(LTRIM(ss.[sales document no])),RTRIM(LTRIM(left(ss.[item no], len(ss.[item no])-1))))
	            End AS OrderItem
		        ,ss.[sales qty] as OrderQuant
                ,ss.[due date] as DueDate
                ,sh.[PO number] as PoNo
                ,sh.[sold-to name1] as Name
                ,si.[material no] as MaterialNo
                ,si.[Item Note] as ItemNote
                FROM [TCB].[dbo].[salesdocscheduleline] ss
                left outer join [salesdocheader] sh on ss.[sales document no] = sh.[sales document no]
                left outer join [salesdocitem] si on si.[sales document no] = ss.[sales document no] and si.[item no] = ss.[item no]
                WHERE " + $" si.plant = '{_factoryCode}' and [due date] BETWEEN  '{startDueDate}' and '{endDueDate}' and left(si.[Item Note],3) <> 'SSS' and si.[reject reason] <> '93' and si.[sales unit] <> 'ST' and (sh.[order type] = 'ZP20' or sh.[order type] = 'ZP30' or sh.[order type] = 'ZP40' or sh.[order type] = 'ZP90') " + configStr;

            //WHERE (Case
            //When [item no] <= 99 Then CONCAT(RTRIM(LTRIM([sales document no])),RTRIM(LTRIM(REVERSE([item no]))))
            //ELSE CONCAT(RTRIM(LTRIM([sales document no])),RTRIM(LTRIM(left([item no], len([item no])-1))))
            //End) IN " + $"('{string.Join("','", OrderItems.ToArray())}')";

            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("SAPConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                //Execute sql query
                moDatasSAP = db.Query<MoData>(queryString).ToList();

                db.Close();
            }

            if (moDatasSAP.Count > 0)
            {
                OrderItems = moDatasSAP.GroupBy(x => x.OrderItem).Select(g => g.First().OrderItem).ToList();
            }

            reportCheckMOAndTextfileSAPViewModel.moDatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDataListBySaleOrdersByDapper(_factoryCode, JsonConvert.SerializeObject(OrderItems), _token));

            reportCheckMOAndTextfileSAPViewModel.moDatas = moDatasSAP.Except(reportCheckMOAndTextfileSAPViewModel.moDatas, new MODataSAPComparer()).OrderBy(m => m.OrderItem).ToList();

            reportCheckMOAndTextfileSAPViewModel.ConfigWordingReport = JsonConvert.DeserializeObject<ConfigWordingReport>(_configWordingReportAPIRepository.GetConfigWordingReportsByFactoryCode(_factoryCode, _token));
        }

        public void SearchReportCheckMOWithSStatusAndKIWI(IConfiguration configuration, ref List<MoDataPrintMastercard> moDatas, string startDueDate, string endDueDate)
        {
            moDatas = JsonConvert.DeserializeObject<List<MoDataPrintMastercard>>(_moDataAPIRepository.GetMoDatasByDueDateRangeAndStatus(_factoryCode, "S", startDueDate, endDueDate, _token));
            var moDatasKIWI = new List<MoDataPrintMastercard>();
            var OrderItems = new List<string>();
            OrderItems = moDatas.GroupBy(x => x.OrderItem).Select(g => g.First().SoKiwi).ToList();

            var queryString = @"SELECT [SalesOrder] as SOKiwi
                FROM [dbo].[LINE_UP]
                WHERE (Plant = " + $"'{_factoryCode}') AND (SalesOrder IN ('{string.Join("','", OrderItems.ToArray())}')) AND (SeriesNo = '1') AND (SeqNo = '1') ORDER BY UpdatedDate DESC";

            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("KIWIConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                //Execute sql query
                moDatasKIWI = db.Query<MoDataPrintMastercard>(queryString).OrderBy(m => m.SoKiwi).ToList();

                db.Close();
            }

            moDatas = moDatas.Intersect(moDatasKIWI, new MODataKIWIComparer()).OrderBy(m => m.SoKiwi).ToList();
        }

        public ConfigWordingReport GetConfigWordingReportByFactoryCode()
        {
            var configWordingReport = new ConfigWordingReport();
            configWordingReport = JsonConvert.DeserializeObject<ConfigWordingReport>(_configWordingReportAPIRepository.GetConfigWordingReportsByFactoryCode(_factoryCode, _token));
            return configWordingReport;
        }

        public ConfigWordingReport CreateConfigWordingReport(string configWordingString)
        {
            var configWordingReport = new ConfigWordingReport();
            configWordingReport = JsonConvert.DeserializeObject<ConfigWordingReport>(_configWordingReportAPIRepository.UpdateConfigWordingReportByFactoryCode(_factoryCode, _username, configWordingString, _token));
            return configWordingReport;
        }

        public void SearchReportMOManual(ref List<CheckRepeatOrder> moDatas, string materialNo, string custName, string pc, string startDueDate, string endDueDate, string startCreateDate, string endCreateDate, string startUpdateDate, string endUpdateDate, string po, string so, string note, string soStatus)
        {
            moDatas = new List<CheckRepeatOrder>();
            moDatas = JsonConvert.DeserializeObject<List<CheckRepeatOrder>>(_moDataAPIRepository.ReportMOManual(_factoryCode, materialNo, custName, pc, startDueDate, endDueDate, startCreateDate, endCreateDate, startUpdateDate, endUpdateDate, po, so, note, soStatus, _token));
        }

        public string GetTIPApiUrlByFactoryCode()
        {
            var interfaceSystemAPI = new InterfaceSystemApi();
            interfaceSystemAPI = JsonConvert.DeserializeObject<InterfaceSystemApi>(interfaceSystemAPIAPIRepository.GetInterfaceSystemAPIsByFactoryCode(_factoryCode, _token));
            if (interfaceSystemAPI != null)
            {
                return interfaceSystemAPI.ApiUrl;
            }

            return null;
        }

        public void SearchReportCheckStatusColor(ref List<CheckStatusColor> statusColors, int colorId)
        {
            statusColors = new List<CheckStatusColor>();
            statusColors = JsonConvert.DeserializeObject<List<CheckStatusColor>>(masterDataAPIRepository.ReportCheckStatusColor(_factoryCode, colorId, _token));
        }
    }

    #region function conpare

    public class MODataKIWIComparer : IEqualityComparer<MoDataPrintMastercard>
    {
        public bool Equals(MoDataPrintMastercard x, MoDataPrintMastercard y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            {
                return false;
            }

            return x.SoKiwi == y.SoKiwi;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(MoDataPrintMastercard moData)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(moData, null))
            {
                return 0;
            }

            //Get hash code for the Code field.
            int hashOrderItem = moData.SoKiwi.GetHashCode();

            //Calculate the hash code for the product.
            return hashOrderItem;
        }
    }

    public class MODataSAPComparer : IEqualityComparer<MoData>
    {
        public bool Equals(MoData x, MoData y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            {
                return false;
            }

            return x.OrderItem == y.OrderItem;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(MoData moData)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(moData, null))
            {
                return 0;
            }

            //Get hash code for the Code field.
            int hashOrderItem = moData.OrderItem.GetHashCode();

            //Calculate the hash code for the product.
            return hashOrderItem;
        }
    }

    #endregion function conpare
}