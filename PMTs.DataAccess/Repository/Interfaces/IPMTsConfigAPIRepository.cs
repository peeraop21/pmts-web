namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPMTsConfigAPIRepository
    {
        string GetPMTsConfigList(string factoryCode, string token);

        string GetSlit(string factoryCode, string token);

        void SavePMTsConfig(string factoryCode, string jsonString, string token);

        void UpdatePMTsConfig(string factoryCode, string jsonString, string token);

        string GetPMTsConfigByFactoryName(string factoryCode, string factoryName, string token);

        //tassanai 11012022
        string GetPmtsConfigByFuncName(string factoryCode, string funcName, string token);

    }
}
