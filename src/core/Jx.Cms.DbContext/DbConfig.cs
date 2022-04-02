using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "数据库类型必须选择")]
        public string DbType { get; set; }

        /// <summary>
        /// 数据库URL
        /// </summary>
        public string DbUrl { get; set; }

        /// <summary>
        /// 数据库端口号
        /// </summary>
        [Range(1, 65535, ErrorMessage = "端口号必须在1-65535之间")]
        public string DbPort { get; set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        [Required(ErrorMessage = "数据库名必须输入")]
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
        [Required(ErrorMessage = "表前缀必须输入")]
        public string Prefix { get; set; }

    }
}