using System;
using System.IO;
using System.Linq;
using FreeSql;
using Furion;
using Jx.Cms.Common.Exceptions;
using Jx.Cms.Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jx.Cms.DbContext
{
    public class DbStartup: AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfigurableOptions<DbConfig>();
            var dbConfig = App.GetOptions<DbConfig>("Db");
            //var dbConfig = Configure.Configuration.GetSection("Db").Get<DbConfig>();
            if (dbConfig != null)
            {
                var ret = SetupDb(services, dbConfig);
                if (!ret.isSuccess)
                {
                    throw new DbException(ret.msg);
                }

                services.Configure<DbConfig>(x => x.CopyFrom(dbConfig));
            }
            else if (File.Exists("install.lock"))
            {
                throw new DbException("数据库配置错误，无数据库配置信息！");
            }
        }

        public static bool CreateTables(DbConfig dbConfig)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
                x.GetTypes().Where(y => y.BaseType != null && y.BaseType.IsGenericType && y.BaseType.GetGenericTypeDefinition() == typeof(BaseEntity<,>)
                                        && y.FullName != null && !y.FullName.Contains("FreeSql")));
            BaseEntity.Orm.CodeFirst.SyncStructure(types.ToArray());
            var filePath = App.WebHostEnvironment.ContentRootFileProvider.GetFileInfo("appsettings.json").PhysicalPath;
            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath));
            var sectionObject = jObject.TryGetValue("Db", out JToken section) ?
                JsonConvert.DeserializeObject<DbConfig>(section.ToString()) : dbConfig;
            jObject["Db"] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
            File.WriteAllText(filePath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
            return true;
        }

        public static (bool isSuccess, string msg) SetupDb(IServiceCollection services, DbConfig dbConfig)
        {
            if (!dbConfig.DbType.IsNullOrEmpty() && Enum.TryParse(dbConfig.DbType, true, out DataType dataType))
            {
                IFreeSql freeSql = null;
                var isDevelopment = App.WebHostEnvironment?.IsDevelopment() ?? true;
                switch (dataType)
                {
                    case DataType.MySql:
                        var connStr = $"data source={dbConfig.DbName};PORT={dbConfig.DbPort};database={dbConfig.DbName}; uid={dbConfig.Username};pwd={dbConfig.Password};";
                        freeSql = new FreeSqlBuilder()
                            .UseAutoSyncStructure(isDevelopment)
                            .UseNoneCommandParameter(true)
                            .UseConnectionString(dataType, connStr)
                            .Build();
                        break;
                    case DataType.SqlServer:
                        break;
                    case DataType.PostgreSQL:
                        break;
                    case DataType.Oracle:
                        break;
                    case DataType.Sqlite:
                        freeSql = new FreeSqlBuilder()
                            .UseAutoSyncStructure(isDevelopment)
                            .UseNoneCommandParameter(true)
                            .UseConnectionString(dataType, $"data source={(dbConfig.DbName.EndsWith(".db")?dbConfig.DbName : dbConfig.DbName + ".db")}")
                            .Build();

                        break;
                    default:
                        return (false, "数据库类型不在指定范围内");
                }

                if (freeSql == null)
                {
                    return (false, "数据库初始化失败");
                }

                if (!freeSql.Ado.ExecuteConnectTest())
                {
                    freeSql.Dispose();
                    return (false, "数据库连接失败");
                }

                freeSql.Aop.ConfigEntity += (s, e) =>
                {
                    e.ModifyResult.Name = dbConfig.Prefix + e.EntityType.Name.Replace("Entity", "").ConvertToUnderLine();
                };

                services.AddSingleton(freeSql);

                BaseEntity.Initialization(freeSql, null);

                return (true, "");
            }
            return (false, "数据库类型不在指定范围内"); ;
        }

    }
}