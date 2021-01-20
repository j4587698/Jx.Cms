using System;
using FreeSql;

namespace Jx.Cms.Entities.Admin
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    public class AdminUserEntity: BaseEntity<AdminUserEntity, Guid>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
    }
}