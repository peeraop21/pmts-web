using PMTs.DataAccess.ModelPresale;
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView
{
    public class PresaleViewModel
    {
        //View
        public string PSM_ID { get; set; }
        public string PSM_Status { get; set; }
        public string Material_No { get; set; }
        public string PC { get; set; }
        public string Cust_Name { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
        public string Board { get; set; }
        public string flute { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public string Plant { get; set; }
        public ParamSearch Param { get; set; }
        public List<PresaleViewModel> PresaleList { get; set; }
        public string PsAwfile { get; set; }
        public string PsDwFile { get; set; }
        public string PsLpFile { get; set; }
        public string PsPoFile { get; set; }
        public string PsQtFile { get; set; }
        public OtherFiles PsOtherFile { get; set; }
        public string ProcessCost { get; set; }
        public string CustComment { get; set; }
        public string MaterialComment { get; set; }
        public string JoinCharacter { get; set; }
        public string PsPvFile { get; set; }
        public string PsAllFile { get; set; }
        public string RoutBarcode { get; set; }

        public class ParamSearch
        {
            public string ddlSearch1 { get; set; }
            public string ddlSearch2 { get; set; }
            public string ddlSearch3 { get; set; }
            public string ddlSearch4 { get; set; }
            public string txtSearch1 { get; set; }
            public string txtSearch2 { get; set; }
            public string txtSearch3 { get; set; }
            public string txtSearch4 { get; set; }
        }

        //DB 
        public PresaleMasterData presaleMasterData { get; set; }
        public List<PresaleRouting> presaleRoutingModels { get; set; }

        public string FlagChange { get; set; }

        public class OtherFiles
        {
            public List<Files> files { get; set; }
            public string file_length { get; set; }
        }
        public class Files
        {
            public string file_url { get; set; }
            public string file_type { get; set; }
        }
    }
}
