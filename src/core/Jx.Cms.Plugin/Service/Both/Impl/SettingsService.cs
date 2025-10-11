using Jx.Cms.Common.Utils;
using Jx.Cms.DbContext.Entities.Settings;

namespace Jx.Cms.Plugin.Service.Both.Impl;

public class SettingsService : ISettingsService
{
    public string GetValue(string type, string name)
    {
        if (!Util.IsInstalled) return string.Empty;
        return SettingsEntity.GetValue(type, name);
    }

    public string GetValue(string name)
    {
        if (!Util.IsInstalled) return string.Empty;
        return SettingsEntity.GetValue(name);
    }

    public void SetValue(string type, string name, string value)
    {
        if (!Util.IsInstalled) return;
        SettingsEntity.SetValue(type, name, value);
    }

    public void SetValue(string name, string value)
    {
        if (!Util.IsInstalled) return;
        SettingsEntity.SetValue(name, value);
    }

    public Dictionary<string, string> GetAllValues(string type)
    {
        if (!Util.IsInstalled) return new Dictionary<string, string>();
        return SettingsEntity.Select.Where(x => x.Type == type).ToDictionary(x => x.Name, y => y.Value);
    }

    public Dictionary<string, string> GetAllValues()
    {
        if (!Util.IsInstalled) return new Dictionary<string, string>();
        return GetAllValues(Constants.SystemType);
    }

    public Dictionary<string, string> GetValuesByNames(IEnumerable<string> names)
    {
        if (!Util.IsInstalled) return new Dictionary<string, string>();
        return GetValuesByNames(Constants.SystemType, names);
    }

    public Dictionary<string, string> GetValuesByNames(string type, IEnumerable<string> names)
    {
        if (!Util.IsInstalled) return new Dictionary<string, string>();
        return SettingsEntity.Where(x => x.Type == type && names.Contains(x.Name))
            .ToDictionary(x => x.Name, y => y.Value);
    }
}