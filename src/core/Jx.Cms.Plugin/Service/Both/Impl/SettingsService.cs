using System.Collections.Generic;
using System.Linq;
using Furion.DependencyInjection;
using Jx.Cms.Common.Utils;
using Jx.Cms.DbContext.Entities.Settings;

namespace Jx.Cms.Plugin.Service.Both.Impl
{
    public class SettingsService: ISettingsService, ITransient
    {
        public string GetValue(string type, string name)
        {
            return SettingsEntity.GetValue(type, name);
        }

        public string GetValue(string name)
        {
            return SettingsEntity.GetValue(name);
        }

        public void SetValue(string type, string name, string value)
        {
            SettingsEntity.SetValue(type, name, value);
        }

        public void SetValue(string name, string value)
        {
            SettingsEntity.SetValue(name, value);
        }

        public Dictionary<string, string> GetAllValues(string type)
        {
            return SettingsEntity.Select.Where(x => x.Type == type).ToDictionary(x => x.Name, y => y.Value);
        }

        public Dictionary<string, string> GetAllValues()
        {
            return GetAllValues(Constants.SystemType);
        }

        public Dictionary<string, string> GetValuesByNames(IEnumerable<string> names)
        {
            return GetValuesByNames(Constants.SystemType, names);
        }

        public Dictionary<string, string> GetValuesByNames(string type, IEnumerable<string> names)
        {
            return SettingsEntity.Where(x => x.Type == type && names.Contains(x.Name)).ToDictionary(x => x.Name, y => y.Value);
        }
    }
}