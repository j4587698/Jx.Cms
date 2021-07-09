using System;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Admin;
using Masuit.Tools.Security;

namespace Jx.Cms.Service.Admin.Impl
{
    public class AdminUserService: ITransient, IAdminUserService
    {
        // 盐
        private string _salt = "E78D376F97CE4A7E89E011FA1FB362F6";

        public bool Register(string username, string password)
        {
            if (AdminUserEntity.Select.Where(x => x.UserName == username).Any())
            {
                return false;
            }

            var admin = new AdminUserEntity()
                {UserName = username, Password = password.MDString2(_salt), Type = "admin"};
            admin.Insert();
            return true;
        }

        public bool LoginCheck(string username, string password)
        {
            Console.WriteLine("123456".MDString2(_salt));
            if (AdminUserEntity.Where(x => x.UserName == username && x.Password == password.MDString2(_salt)).Count() == 1)
            {
                return true;
            }

            return false;
        }
    }
}