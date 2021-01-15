using System.ComponentModel.DataAnnotations;

namespace Jx.Cms.Admin.ViewModel
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
        public string AdminName { get; set; }

        /// <summary>
        /// 管理员密码
        /// </summary>
        [Required(ErrorMessage = "管理员密码必须输入")]
        public string AdminPassword { get; set; }

        /// <summary>
        /// 管理员密码重复
        /// </summary>
        [Required(ErrorMessage = "管理员密码必须输入")]
        public string AdminRePassword { get; set; }
    }
}