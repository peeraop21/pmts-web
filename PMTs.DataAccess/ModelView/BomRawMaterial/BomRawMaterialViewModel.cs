using Microsoft.AspNetCore.Mvc.Rendering;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.BomRawMaterial
{
    public class BomRawMaterialViewModel
    {
        public BomRawMaterialViewModel()
        {
            MasterDataList = new List<MasterData>();
            PPCRawMaterialMastersList = new List<PpcRawMaterialMaster>();
        }
        public IEnumerable<SelectListItem> Lst_FGMaterial { get; set; }
        public IEnumerable<SelectListItem> Lst_UnitOfMeasureCode { get; set; }
        public IEnumerable<SelectListItem> Lst_Status { get; set; }
        public GeneralViewModel GeneralViewModel { get; set; }
        public List<MasterData> MasterDataList { get; set; }
        public List<PpcRawMaterialMaster> PPCRawMaterialMastersList { get; set; }
        public List<PpcRawMaterialProductionBom> PPCRawMaterialProductionBomList { get; set; }

        public string MaterialNoSearch { get; set; }

    }
    public class GeneralViewModel
    {
        public string FGMaterial { get; set; }
        public string UnitOfMeasureCode { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string CreationBy { get; set; }
        public string CreationDate { get; set; }
        public string LastUserModified { get; set; }
        public string LastDateModified { get; set; }
    }

    public class RawMaterialLineFront
    {
        public int Id { get; set; }
        public int? MaterialType { get; set; }
        public string MaterialNumber { get; set; }
        public string MaterialDescription { get; set; }
        public decimal? NetWeight { get; set; } //weight per
        public string MaterialGroup { get; set; } // category
        public string Uom { get; set; } // Measure of unit
        public decimal? Width { get; set; } //ขนาดผลิต(กว้าง)
        public decimal? Length { get; set; } //ขนาดผลิต(สูง)
        public int? Lay { get; set; } //จำนวนตัวลง
        public int? CutSize { get; set; } //ไซส์ตัด
        public DateTime? UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public string OldMaterialNumber { get; set; }
        public string Plant { get; set; }
        public string SizeUom { get; set; }
        public decimal? BomAmount { get; set; }
    }
    public class RawMaterialLineRequest
    {
        public RawMaterialLineRequest()
        {
            BomRawData = new List<RawMaterialLineFront>();
        }
        //public int Id { get; set; }
        public string FgMaterial { get; set; }
        public List<RawMaterialLineFront> BomRawData { get; set; }
    }



}
