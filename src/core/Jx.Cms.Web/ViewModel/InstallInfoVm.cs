using System.ComponentModel.DataAnnotations;

namespace Jx.Cms.Web.ViewModel
{
    /// <summary>
    /// 安装信息
    /// </summary>
    public class InstallInfoVm
    {
        /// <summary>
        /// 管理员用户名
        /// </summary>
        [Required(ErrorMessage = "管理员用户名必须输入")]
        public string UserName { get; set; }

        /// <summary>
        /// 管理员密码
        /// </summary>
        [Required(ErrorMessage = "管理员密码必须输入")]
        public string Password { get; set; }

        /// <summary>
        /// 管理员密码重复
        /// </summary>
        [Required(ErrorMessage = "管理员密码必须输入")]
        public string RePassword { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        [Required(ErrorMessage = "用户邮箱必须输入")]
        public string Email { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        public string NickName { get; set; }
    }
}