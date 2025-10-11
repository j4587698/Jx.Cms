using System;
using System.IO;
using System.Linq;
using FreeSql;
using Jx.Cms.Common.Exceptions;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Common.Utils;
using Jx.Toolbox.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Jx.Cms.DbContext
{
    public class DbStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfigurableOptions<DbConfig>();
            // DbConfig会在Startup.cs中配置
        }

        public static bool CreateTables(DbConfig dbConfig)
        {
            try
            {
                var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
                    x.GetTypes().Where(y => y.BaseType != null && y.BaseType.IsGenericType && y.BaseType.GetGenericTypeDefinition() == typeof(BaseEntity<,>)
                                            && y.FullName != null && !y.FullName.Contains("FreeSql")));
                BaseEntity.Orm.CodeFirst.SyncStructure(types.ToArray());
                var filePath = Path.Combine(AppContext.BaseDirectory, "config", "dbsettings.json");
            var jObject = File.Exists(filePath) ? JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath)) : new JObject();
            jObject["Db"] = JObject.Parse(JsonConvert.SerializeObject(dbConfig));
            File.WriteAllText(filePath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
            }
            catch (Exception e)
            {
                Log.Error(e, "建表出错了");
                return false;
            }
            
            return true;
        }

        public static (bool isSuccess, string msg) SetupDb(IServiceCollection services, DbConfig dbConfig)
        {
            if (!dbConfig.DbType.IsNullOrEmpty() && Enum.TryParse(dbConfig.DbType, true, out DataType dataType))
            {
                var isDevelopment = true;
                string connStr = "";
                switch (dataType)
                {
                    case DataType.MySql:
                        connStr = $"Data Source={dbConfig.DbUrl};Port={dbConfig.DbPort};User ID={dbConfig.Username};Password={dbConfig.Password}; Initial Catalog={dbConfig.DbName};Charset=utf8; SslMode=none;Min pool size=1";
                        break;
                    case DataType.SqlServer:
                        connStr = $"Data Source={dbConfig.DbUrl},{dbConfig.DbPort};User Id={dbConfig.Username};Password={dbConfig.Password};Initial Catalog={dbConfig.DbName};TrustServerCertificate=true;Pooling=true;Min Pool Size=1";
                        break;
                    case DataType.PostgreSQL:
                        connStr = $"Host={dbConfig.DbUrl};Port={dbConfig.DbPort};Username={dbConfig.Username};Password={dbConfig.Password}; Database={dbConfig.DbName};Pooling=true;Minimum Pool Size=1";
                        break;
                    case DataType.Oracle:
                        connStr = $"user id={dbConfig.Username};password={dbConfig.Password}; data source=//{dbConfig.DbUrl}:{dbConfig.DbPort}/{dbConfig.DbName};Pooling=true;Min Pool Size=1";
                        break;
                    case DataType.Sqlite:
                        var path = Path.GetDirectoryName(dbConfig.DbName);
                        if (!path.IsNullOrEmpty() && !Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        connStr = $"data source={(dbConfig.DbName.EndsWith(".db")?dbConfig.DbName : dbConfig.DbName + ".db")}";
                        break;
                    default:
                        Log.Error("数据库类型不在指定范围内");
                        return (false, "数据库类型不在指定范围内");
                }
                var freeSql = new FreeSqlBuilder()
                    .UseAutoSyncStructure(isDevelopment)
                    .UseNoneCommandParameter(true)
                    .UseConnectionString(dataType, connStr)
                    .Build();
                
                if (freeSql == null)
                {
                    Log.Error("数据库初始化失败");
                    return (false, "数据库初始化失败");
                }

                if (!freeSql.Ado.ExecuteConnectTest())
                {
                    Log.Error("数据库连接失败");
                    freeSql.Dispose();
                    return (false, "数据库连接失败");
                }

                freeSql.Aop.ConfigEntity += (s, e) =>
                {
                    e.ModifyResult.Name = dbConfig.Prefix + e.EntityType.Name.Replace("Entity", "").ToUnderLine();
                };

                services.AddSingleton(freeSql);

                BaseEntity.Initialization(freeSql, null);

                return (true, "");
            }
            Log.Error("数据库类型不在指定范围内");
            return (false, "数据库类型不在指定范围内");
        }

    }
}