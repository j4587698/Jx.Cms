using System;
using System.Collections.Generic;
using System.Linq;
using Furion.DependencyInjection;
using Jx.Cms.Common.Attribute;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Service.Admin.Impl
{
    /// <summary>
    /// 菜单相关
    /// </summary>
    public class MenuService: IMenuService, ITransient
    {
        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <returns>菜单列表</returns>
        public List<MenuAttribute> GetAllMenu()
        {
            List<MenuAttribute> menuAttributes = new List<MenuAttribute>();
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().AsEnumerable()
                .Where(type => typeof(ComponentBase).IsAssignableFrom(type)));
            foreach (var type in types)
            {
                if (type.GetCustomAttributes(false).FirstOrDefault(x => x is MenuAttribute) is MenuAttribute m)
                {
                    menuAttributes.Add(m);
                }
            }
            return menuAttributes;
        }
    }
}