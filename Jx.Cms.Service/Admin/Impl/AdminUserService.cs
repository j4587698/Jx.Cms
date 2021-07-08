using Furion.DependencyInjection;
using Jx.Cms.Entities.Admin;

namespace Jx.Cms.Service.Admin.Impl
{
    public class AdminUserService: ITransient, IAdminUserService
    {
        public bool LoginCheck(string username, string password)
        {
            if (AdminUserEntity.Where(x => x.UserName == username || x.Password == password).Count() == 1)
            {
                return true;
            }

            return false;
        }
    }
}