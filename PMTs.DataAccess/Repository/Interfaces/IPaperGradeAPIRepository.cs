namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPaperGradeAPIRepository
    {
        string GetPaperGradeList(string factoryCode, string token);

        void SavePaperGrade(string jsonString, string token);

        void UpdatePaperGrade(string jsonString, string token);

        void DeletePaperGrade(string jsonString, string token);

        string GetPaperGradeByGradeAndActive(string factoryCode, string grade, string token);

        string GetPaperGradeByGrade(string factoryCode, string grade, string token);

        string GetPaperGradesWithGradeCodeMachine(string factoryCode, string token);

        void SavePaperGradeWithGradeCodeMachine(string factoryCode, string jsonString, string token);
        void UpdatePaperGradeWithGradeCodeMachine(string factoryCode, string jsonString, string token);
        string GetGradeList(string factoryCode, string token);
    }
}
