namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IStandardPatternNameAPIRepository
    {
        string GetStandardPatternName(string _factoryCode, string PictureNamePallet, string token);
        string GetAllByFactory(string _factoryCode, string token);
        string GetAll(string _factoryCode, string token);
        string GetCalculatePallet(string factoryCode, string palletCalculateParam, string token);


    }
}
