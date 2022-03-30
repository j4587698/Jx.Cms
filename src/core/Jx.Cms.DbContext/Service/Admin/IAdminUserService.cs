using Jx.Cms.DbContext.Entities.Admin;

namespace Jx.Cms.DbContext.Service.Admin
{
    public interface IAdminUserService
    {
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="adminUserEntity">用户信息</param>
        /// <returns></returns>
        public bool Register(AdminUserEntity adminUserEntity);
        
        /// <summary>
        /// 登录检查
        /// </summary>
        /// <param name="username">用户名或邮箱</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public AdminUserEntity Login(string username, string password);

        /// <summary>
        /// 根据用户名获取用户信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public AdminUserEntity GetUserByUserName(string username);
    }
}