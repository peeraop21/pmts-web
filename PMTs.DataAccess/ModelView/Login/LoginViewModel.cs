namespace PMTs.DataAccess.ModelView.Login
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserDomain { get; set; }
        public string SaleOrg { get; set; }
        public string PlantCode { get; set; }
        public int DefaultRoleId { get; set; }
        public string PictureUser { get; set; }
        public string BusinessGroup { get; set; } //Tassanai update 12052022

    }

    public class UserSessionModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserDomain { get; set; }
        public string SaleOrg { get; set; }
        public string FactoryCode { get; set; }
        public int DefaultRoleId { get; set; }
        public string FirstNameTh { get; set; }
        public string Telephone { get; set; }

        //Tassanai Update 05022020 ======
        public string LastNameTh { get; set; }
        public string FirstNameEn { get; set; }
        public string LastNameEn { get; set; }
        public string Email { get; set; }

        //Tassanai Update 05022020 ======

        //boo Update 10042020
        public string Token { get; set; }

        //Tassanai Update 29052020
        public string PictureUser { get; set; }

        public string BusinessGroup { get; set; } //Tassanai update 12052022
        public string PlanningProgram { get; set; }

    }
    public class UserADSCG
    {
        public string name { get; set; }
        public string givenName { get; set; }
        public string middleName { get; set; }
        public string sn { get; set; }
        public string displayName { get; set; }
        public string description { get; set; }
        public string physicalDeliveryOfficeName { get; set; }
        public string mail { get; set; }
        public string sAMAcountName { get; set; }
        public string title { get; set; }
        public string department { get; set; }
        public string company { get; set; }
        public string manager { get; set; }
        public string postOfficeBox { get; set; }
        public string pager { get; set; }
        public string memberOf { get; set; }
        public string userPrincipalName { get; set; }
        public string SCGCostCenter { get; set; }
        public string SCGComCodeBilling { get; set; }
        public string employeeType { get; set; }
    }
    public class UserTIP
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string UrlApi { get; set; }
        public string Token { get; set; }
    }
}
