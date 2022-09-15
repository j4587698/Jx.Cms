using Furion.DependencyInjection;
using Jx.Cms.DbContext.Entities.Admin;
using Jx.Toolbox.Cryptography;
using Jx.Toolbox.Extensions;

namespace Jx.Cms.Plugin.Service.Admin.Impl
{
    public class AdminUserService: ITransient, IAdminUserService
    {
        // 盐
        private string _salt = "E78D376F97CE4A7E89E011FA1FB362F6";

        public bool Register(AdminUserEntity adminUserEntity)
        {
            if (AdminUserEntity.Select.Where(x => x.UserName == adminUserEntity.UserName).Any())
            {
                return false;
            }

            adminUserEntity.Password = MD5.MD5String2WithSalt(adminUserEntity.Password, _salt);
            adminUserEntity.Insert();
            return true;
        }

        public AdminUserEntity Login(string username, string password)
        {
            var passwordMd5 = MD5.MD5String2WithSalt(password, _salt);
            var entity = AdminUserEntity.Where(x =>
                (x.UserName == username || x.Email == username) && x.Password == passwordMd5);
            if (entity.Count() == 1)
            {
                return entity.First();
            }

            return null;
        }

        public AdminUserEntity GetUserByUserName(string username)
        {
            var entity = AdminUserEntity.Where(x => x.UserName == username).First();
            if (entity == null)
            {
                return null;
            }

            if (entity.NickName.IsNullOrEmpty())
            {
                entity.NickName = entity.UserName;
            }

            return entity;
        }
    }
}