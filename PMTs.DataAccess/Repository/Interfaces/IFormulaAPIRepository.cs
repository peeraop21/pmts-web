namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IFormulaAPIRepository
    {
        string CalculateListRouting(string factoryCode, string model, string token);
        string CalculateRouting(string factoryCode, string Machine, string Flut, string CutSheetwid, string Material, string token);
        string CalculateRoutingByCut(string factoryCode, string Cut, string WidthIn, string Flut, string materialNo, string machine, string token);
        string CalculateRoutingByPaperWidth(string factoryCode, string PaperWidth, string WidthIn, string Flut, string cut, string token);
        string CalculateMoTargetQuantity(string factoryCode, string orderQuant, string toleranceOver, string flute, string materialNo, string cut, string token);
        string CalculateRSC(string factoryCode, string modelRSC, string token);
        string CalculateRSC1Piece(string factoryCode, string modelRSC, string token);
        string CalculateRSC2Piece(string factoryCode, string modelRSC, string token);
        string CalculateDC(string factoryCode, string modelRSC, string token);
        string CalculateSF(string factoryCode, string modelRSC, string token);
        string CalculateHC(string factoryCode, string modelRSC, string token);
        string CalculateHB(string factoryCode, string modelRSC, string token);
        string CalculateCG(string factoryCode, string modelRSC, string token);
        string CalculateAC(string factoryCode, string modelRSC, string token);
        //string ReCalculateTrim(string factoryCode, string flute,string action, string user, string token);
        string GetReCalculateTrim(string factoryCode, string flute, string machine, string boxType, string printMethod, string proType, string user, string token);
        void SaveReCalculateTrim(string factoryCode, string jsonString, string token);
        string GetCalculateMoTargetQuantityOffset(string factoryCode, string orderQuant, string materialNo, string orderItem, string userName, string token);
        string CalSizeDimensions(string factoryCode, string materialNo, string token);
    }
}
