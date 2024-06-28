namespace PMTs.DataAccess.ModelView
{
    public class SessionsModel
    {
        public string UserName { get; set; }
        public string PlantCode { get; set; }
        public string SaleOrg { get; set; }
        public int USERID { get; set; }
        public int ROLEID { get; set; }
        public string Material_No { get; set; }
    }

    public class Transessions_PCust
    {
        public int? CustTmpId { get; set; }
        public string CustName { get; set; }
        public string Material_No { get; set; }
        public int? ProdCust_Status { get; set; }
    }




}
