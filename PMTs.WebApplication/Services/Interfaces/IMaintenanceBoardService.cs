using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceBoard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceBoardService
    {
        //MaintenanceBoardModel GetBoard(MaintenanceBoardModel maintenanceBoardModel);
        //MaintenanceBoardModel GetSelectFlute(MaintenanceBoardModel maintenanceBoardModel);
        BoardCombindMainTainModel getFristBoardCombind();
        BoardCombindMainTainModel getMaxcodeBoardCombind();
        bool AddBoardcombind(BoardCombindMainTainModel model);
        void UpdateBoardcombind(BoardCombindMainTainModel model);
        BoardCombindMainTainModel getAllBoardspectByCode(string code);
        BoardCombindMainTainModel getBoardCombindAccCode(string code);
        void ManageBoardcombindAcc(List<BoardCombindAccUpdate> model);
        string GenerateBoardCombindCode();
        string GenerateFileDataForSAP(ExportDataForSAPRequest request);
    }
}
