using System.Collections.Generic;
using System.Linq;
using Furion.DependencyInjection;
using Jx.Cms.Common.Utils;
using Jx.Cms.DbContext.Entities.Settings;
using Jx.Cms.Plugin.Utils;

namespace Jx.Cms.Plugin.Service.Impl
{
    public class PluginService: IPluginService, ITransient
    {
        public List<PluginConfig> GetAllPlugins()
        {
            return PluginUtil.GetAllPlugins();
        }

        public PluginConfig GetPluginById(int id)
        {
            var pluginEntity = PluginEntity.Find(id);
            return GetPluginByPluginId(pluginEntity.PluginId);
        }

        public PluginConfig GetPluginByPluginId(string pluginId)
        {
            return GetAllPlugins().FirstOrDefault(x => x.PluginId == pluginId);
        }

        public bool ChangePluginStatus(string pluginId)
        {
            return PluginUtil.ChangePluginStatus(pluginId);
        }

        public bool ChangePluginStatus(int id)
        {
            var pluginEntity = PluginEntity.Find(id);
            return pluginEntity != null && ChangePluginStatus(pluginEntity.PluginId);
        }

        public bool DeletePlugin(string pluginId)
        {
            var result = PluginUtil.DeletePlugin(pluginId);
            if (result)
            {
                if (PluginEntity.Select.Where(x => x.PluginId == pluginId).ToDelete().ExecuteAffrows() == 0)
                {
                    return false;
                }
            }

            return result;
        }

        public bool DeletePlugin(int id)
        {
            var plugin = PluginEntity.Find(id);
            return plugin != null && DeletePlugin(plugin.PluginId);
        }
    }
}