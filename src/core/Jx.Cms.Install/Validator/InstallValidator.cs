﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BootstrapBlazor.Components;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext;
using Jx.Cms.Install.ViewModel;
using Jx.Toolbox.Extensions;

namespace Jx.Cms.Install.Validator
{
    /// <summary>
    /// 
    /// </summary>
    public class InstallValidator: ValidatorBase 
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
                    case nameof(infoVm.RePassword):
                        if (infoVm.RePassword != infoVm.Password)
                        {
                            results.Add(new ValidationResult("两次密码不一致", new []{context.MemberName}));
                        }
                        break;
                }
            }
        }
    }
}