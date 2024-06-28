using System;

namespace PMTs.DataAccess.ModelView
{
    public class PrintMethodViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Method { get; set; }
        public int? AmountColor { get; set; }
    }

    public class JointViewModel
    {
        public int Id { get; set; }
        public string JointId { get; set; }
        public string JointName { get; set; }
        public string JointDescription { get; set; }
    }

    //Tassanai update 11/03/2020
    public class ProductPropStandardPatternName
    {
        public int Id { get; set; }
        public string PatternName { get; set; }
        public string PictureNamePallet { get; set; }
        public string Picturepath { get; set; }
        public bool? Status { get; set; }

    }

    public class JoinCharacterViewModel
    {
        public int Id { get; set; }
        public string NameJoinCharacter { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }


}
