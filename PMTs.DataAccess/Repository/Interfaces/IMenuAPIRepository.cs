namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMenuAPIRepository
    {
        dynamic GetMenuList(int roleid);
        dynamic GetSubMenuList();



    }
}
