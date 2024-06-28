using Microsoft.AspNetCore.Http;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.NewProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IProductSpecService
    {
        void GetProductSpec(ref TransactionDataModel transactionDataModel, string action);

        int chkPriority(ProductSpecViewModel model);

        void Coating(string CoatingArray, TransactionDataModel modelx);

        string GetBoardName(ProductSpecViewModel board);

        //string GetBoardKIWI(string boardCombine);

        double? GetBasisWeight(string code, string flute, string factoryCode);

        List<BoardViewModel> GetBoardAlt(string prefix, string flute);

        List<SearchBoardAlt> SearchBoardAlt(string flute);

        List<BoardSpecWeight> GetBoardSpec(string code, string flute, string PlantCode);

        BoardCombine GetBoardByCode(string code);

        ProductSpecViewModel AddBoardAlt(ProductSpecViewModel model);

        ProductSpecViewModel RemoveBoardAlt(int prior);

        ProductSpecViewModel ShowBoardAlt();

        ProductSpecViewModel SortBoardAlt(TransactionDataModel model, int seqNo, string action);

        TransactionDataModel SaveDataToModel_ProductSpec(TransactionDataModel temp, IFormFile printMaster);

        string SaveProductSpec(ProductSpecViewModel board, string eventFlag, int countMat, ProductInfoView info, int idPDT);

        void SaveBoardUse(ProductSpecViewModel board);

        void UpdateCostIntoPlantView(TransactionDataModel model);

        void RemoveBoardAlt(string mat);

        void AddBoardAltToDatabase(ProductSpecViewModel board);

        //decimal ConvertmmToInch(int? mm);

        List<ProductSpecViewModel> ComputeRSC(ProductSpecViewModel model);

        //ProductSpecViewModel ComputeRSC(ProductSpecViewModel model);

        List<ProductSpecViewModel> ComputeOneP(ProductSpecViewModel model);

        List<ProductSpecViewModel> ComputeTwoP(ProductSpecViewModel model);

        List<ProductSpecViewModel> ComputeDC(ProductSpecViewModel model);

        List<ProductSpecViewModel> ComputeSF(ProductSpecViewModel model);

        List<ProductSpecViewModel> ComputeHC(ProductSpecViewModel model);

        List<ProductSpecViewModel> ComputeHB(ProductSpecViewModel model);

        List<ProductSpecViewModel> ComputeCG(ProductSpecViewModel model);

        //List<ProductSpecViewModel> ComputeOther(ProductSpecViewModel model);

        List<ProductSpecViewModel> ComputeTeeth(ProductSpecViewModel model);

        List<ProductSpecViewModel> ComputeWeightAndArea(ProductSpecViewModel model);

        void SetPicData(string[] Base64);

        List<ProductSpecViewModel> GetBoard(ProductSpecViewModel model);

        ProductSpecViewModel CriteriaBoardFromThickness(ProductSpecViewModel model);

        ProductSpecViewModel CriteriaBoardHoney(ProductSpecViewModel model);
        List<Coating> Coatinglst(TransactionDataModel model);

    }
}
