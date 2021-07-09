namespace Jx.Cms.Service.Admin
{
    public interface IAdminUserService
    {

        public bool Register(string username, string password);
        
        public bool LoginCheck(string username, string password);
    }
}