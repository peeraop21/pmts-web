namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IRoutingAPIRepository
    {
        string GetRoutingList(string factoryCode, string token);

        string GetRoutingsByMaterialNo(string factoryCode, string materialNo, string token);

        string GetRoutingByMaterialNoAndMachine(string factoryCode, string materialNo, string machine, string token);

        void SaveRouting(string factoryCode, string materialNo, string jsonString, string token);

        void UpdateRouting(string factoryCode, string jsonString, string token);
        void Update1RowOfRouting(string factoryCode, string jsonString, string token);

        void DeleteRouting(string factoryCode, string jsonString, string token);

        string GetNumberOfRoutingByShipBlk(string factoryCode, string materialNo, bool semiBlk, string token);
        void UpdateRoutingPDISStatus(string factoryCode, string Material, string Status, string token);
        void DeleteRoutingByMaterialNoAndFactory(string factoryCode, string materialNo, string token);
        void DeleteRoutingByMaterialNoAndFactoryAndSeq(string factoryCode, string materialNo, string Seq, string token);
        string GetDapperRoutingByMatList(string factoryCode, string Condition, string token);
        string GetRoutingsByMaterialNos(string factoryCode, string materialNos, string token);
        void UpdateRoutings(string factoryCode, string routingsJson, string token);
        string UpdateReCalculateTrimFromFile(string factoryCode, string reCalculateTrimJson, string token);
        string GetRoutingsByMaterialNoContain(string factoryCode, string materialNo, string token);
    }
}
