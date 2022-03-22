using System;
using System.ComponentModel;
using FreeSql;

namespace Jx.Cms.DbContext.Entities.Admin
{
    [Description("系统用户信息表")]
    public class AdminUserEntity: BaseEntity<AdminUserEntity, Guid>
    {
        [Description("用户名")]
        public string UserName { get; set; }

        [Description("密码")]
        public string Password { get; set; }

        [Description("邮箱")]
        public string Email { get; set; }

        [Description("显示名")]
        public string NickName { get; set; }

        [Description("用户主页")]
        public string HomePage { get; set; }
        
        [Description("用户类型")]
        public string Type { get; set; }
    }
}