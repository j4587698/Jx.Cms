using Furion.ConfigurableOptions;

namespace Jx.Cms.DbContext
{
    /// <summary>
    /// 数据库配置类
    /// </summary>
    [OptionsSettings("Db")]
    public class DbConfig : IConfigurableOptions
    {
        /// <summary>
        /// 数据库类型，目前支持sqlite,mysql,sqlserver,oracle,postgresql
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// 数据库URL
        /// </summary>
        public string DbUrl { get; set; }

        /// <summary>
        /// 数据库端口号
        /// </summary>
        public string DbPort { get; set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 数据库用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 数据库密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 表前缀
        /// </summary>
        public string Prefix { get; set; }
        
    }
}