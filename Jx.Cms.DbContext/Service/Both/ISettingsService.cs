using System.Collections.Generic;

namespace Jx.Cms.DbContext.Service.Both
{
    public interface ISettingsService
    {
        /// <summary>
        /// 根据类型名与设置项名获取值
        /// </summary>
        /// <param name="type">类型名</param>
        /// <param name="name">设置项名</param>
        /// <returns>设置项值</returns>
        string GetValue(string type, string name);

        /// <summary>
        /// 根据设置项名获取系统类型设置项的值（type=="system"）
        /// </summary>
        /// <param name="name">设置项名</param>
        /// <returns>设置项值</returns>
        string GetValue(string name);

        /// <summary>
        /// 设置设置项的值
        /// </summary>
        /// <param name="type">设置项类型</param>
        /// <param name="name">设置项名</param>
        /// <param name="value">设置项值</param>
        void SetValue(string type, string name, string value);

        /// <summary>
        /// 设置系统设置项的值(type=="system")
        /// </summary>
        /// <param name="name">设置项名</param>
        /// <param name="value">设置项值</param>
        void SetValue(string name, string value);

        /// <summary>
        /// 获取类型下所有设置项的值
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>设置项值列表</returns>
        Dictionary<string, string> GetAllValues(string type);

        /// <summary>
        /// 获取system类型下所有设置项的值
        /// </summary>
        /// <returns>设置项列值表</returns>
        Dictionary<string, string> GetAllValues();

        /// <summary>
        /// 获取system类型下指定设置项名列表的值
        /// </summary>
        /// <param name="names">设置项名列表</param>
        /// <returns>设置项列值表</returns>
        Dictionary<string, string> GetValuesByNames(IEnumerable<string> names);

        /// <summary>
        /// 获取类型下指定设置项名列表的值
        /// </summary>
        /// <param name="type">类型名</param>
        /// <param name="names">设置项名列表</param>
        /// <returns>设置项值列表</returns>
        Dictionary<string, string> GetValuesByNames(string type, IEnumerable<string> names);
    }
}