namespace PMTs.DataAccess.ModelView.Login
{
    public class SessionModel
    {
        public int Id { get; set; }
        public int DefaultRoleId { get; set; }
        public string Username { get; set; }
        public string SaleORG { get; set; }
        public string PlantCode { get; set; }

        // Menu
        //   public dynamic MenusDataList { get; set; }

    }
}
