using System;
using System.Collections.Generic;
using System.Linq;
using BootstrapBlazor.Components;
using Furion.DependencyInjection;
using Jx.Cms.Admin.Attribute;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Admin.Service.Impl
{
    public class MenuService: IMenuService, ITransient
    {
        public List<MenuItem> GetAllMenu()
        {
            List<MenuItem> menus = new List<MenuItem>();
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

            foreach (var attribute in menuAttributes.Where(x => x.ParentId == "").OrderByDescending(x => x.Order))
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Icon = attribute.IconClass;
                menuItem.Text = attribute.DisplayName;
                menuItem.Url = attribute.Path;
                GenerateMenu(menuItem, attribute.Id, menuAttributes);
                menus.Add(menuItem);
            }
            return menus;
        }

        private void GenerateMenu(MenuItem item, string parentId, List<MenuAttribute> menuAttributes)
        {
            var menus = menuAttributes.Where(x => x.ParentId == parentId).OrderByDescending(x => x.Order);
            foreach (var menuAttribute in menus)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Icon = menuAttribute.IconClass;
                menuItem.Text = menuAttribute.DisplayName;
                menuItem.Url = menuAttribute.Path;
                if (menuAttributes.Any(x => x.ParentId == menuAttribute.Id))
                {
                    GenerateMenu(menuItem, menuAttribute.Id, menuAttributes);
                }
                item.AddItem(menuItem);
            }
        }
    }
}