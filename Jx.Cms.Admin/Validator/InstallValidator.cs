using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BootstrapBlazor.Components;
using Jx.Cms.Admin.ViewModel;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext;
using Masuit.Tools;

namespace Jx.Cms.Admin.Validator
{
    /// <summary>
    /// 
    /// </summary>
    public class InstallValidator: ValidatorComponentBase
    {
        public override void Validate(object propertyValue, ValidationContext context, List<ValidationResult> results)
        {
            if (context.ObjectInstance is DbConfig dbConfig)
            {
                if (dbConfig.DbType != DbTypeEnum.Sqlite.ToString())
                {
                    switch (context.MemberName)
                    {
                        case nameof(dbConfig.DbUrl):
                            if (dbConfig.DbUrl.IsNullOrEmpty())
                            {
                                results.Add(new ValidationResult("数据库URL不能为空", new []{context.MemberName}));
                            }
                            break;
                        case nameof(dbConfig.DbPort):
                            if (dbConfig.DbPort.IsNullOrEmpty())
                            {
                                results.Add(new ValidationResult("数据库端口号不能为空", new []{context.MemberName}));
                            }
                            break;
                        case nameof(dbConfig.Username):
                            if (dbConfig.Username.IsNullOrEmpty())
                            {
                                results.Add(new ValidationResult("数据库用户名不能为空", new []{context.MemberName}));
                            }
                            break;
                        case nameof(dbConfig.Password):
                            if (dbConfig.Password.IsNullOrEmpty())
                            {
                                results.Add(new ValidationResult("数据库用户密码不能为空", new []{context.MemberName}));
                            }
                            break;
                    }
                }
            }
            else if (context.ObjectInstance is InstallInfoVm infoVm)
            {
                switch (context.MemberName)
                {
                    case nameof(infoVm.AdminRePassword):
                        if (infoVm.AdminRePassword != infoVm.AdminPassword)
                        {
                            results.Add(new ValidationResult("两次密码不一致", new []{context.MemberName}));
                        }
                        break;
                }
            }
        }
    }
}