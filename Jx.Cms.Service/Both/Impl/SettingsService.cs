using System;
using System.Collections.Generic;
using System.Linq;
using Furion.DependencyInjection;
using Jx.Cms.Common.Utils;
using Jx.Cms.Entities.Settings;

namespace Jx.Cms.Service.Both.Impl
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

        public IEnumerable<(string name, string value)> GetAllValues(string type)
        {
            return SettingsEntity.Select.Where(x => x.Type == type).ToList(x => new ValueTuple<string, string>(x.Name, x.Value));
        }

        public IEnumerable<(string name, string value)> GetAllValues()
        {
            return GetAllValues(SettingsConstants.SystemType);
        }

        public IEnumerable<(string name, string value)> GetValuesByNames(IEnumerable<string> names)
        {
            return GetValuesByNames(SettingsConstants.SystemType, names);
        }

        public IEnumerable<(string name, string value)> GetValuesByNames(string type, IEnumerable<string> names)
        {
            return SettingsEntity.Where(x => x.Type == type && names.Contains(x.Name)).ToList(x => new ValueTuple<string, string>(x.Name, x.Value));
        }
    }
}