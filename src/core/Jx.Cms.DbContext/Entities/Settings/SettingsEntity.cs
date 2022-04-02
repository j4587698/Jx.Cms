using System;
using System.ComponentModel;
using FreeSql;
using Jx.Cms.Common.Utils;

namespace Jx.Cms.DbContext.Entities.Settings
{
    [Description("设置表")]
    public class SettingsEntity: BaseEntity<SettingsEntity, Guid>
    {
        /// <summary>
        /// 关联类型
        /// </summary>
        [Description("关联类型")]
        public string Type { get; set; }

        /// <summary>
        /// 设置项名
        /// </summary>
        [Description("设置项名")]
        public string Name { get; set; }

        /// <summary>
        /// 设置项的值
        /// </summary>
        [Description("设置项的值")]
        public string Value { get; set; }

        /// <summary>
        /// 根据类型名与设置项名获取值
        /// </summary>
        /// <param name="type">类型名</param>
        /// <param name="name">设置项名</param>
        /// <returns>设置项值</returns>
        public static string GetValue(string type, string name)
        {
            return Where(x => x.Name == name && x.Type == type).First()?.Value;
        }

        /// <summary>
        /// 根据设置项名获取系统类型设置项的值（type=="system"）
        /// </summary>
        /// <param name="name">设置项名</param>
        /// <returns>设置项值</returns>
        public static string GetValue(string name)
        {
            return GetValue(Constants.SystemType, name);
        }

        /// <summary>
        /// 设置设置项的值
        /// </summary>
        /// <param name="type">设置项类型</param>
        /// <param name="name">设置项名</param>
        /// <param name="value">设置项值</param>
        public static void SetValue(string type, string name, string value)
        {
            var settings = Where(x => x.Name == name && x.Type == type).First()
                           ?? new SettingsEntity();
            settings.Name = name;
            settings.Type = type;
            settings.Value = value;
            settings.Save();
        }

        /// <summary>
        /// 设置系统设置项的值(type=="system")
        /// </summary>
        /// <param name="name">设置项名</param>
        /// <param name="value">设置项值</param>
        public static void SetValue(string name, string value)
        {
            SetValue(Constants.SystemType, name, value);
        }
    }
}