namespace Jx.Cms.Service.Admin
{
    public interface IAdminUserService
    {
        public bool LoginCheck(string username, string password);
    }
}