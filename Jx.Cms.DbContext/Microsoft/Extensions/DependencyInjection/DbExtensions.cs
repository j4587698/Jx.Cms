using System;
using System.IO;
using System.Linq;
using FreeSql;
using Jx.Cms.Common.Configure;
using Jx.Cms.Common.Exceptions;
using Jx.Cms.Common.Extensions;
using Jx.Cms.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbExtensions
    {
        public static IServiceCollection AddFreeSql(this IServiceCollection services)
        {
            var dbConfig = Configure.Configuration.GetSection("Db").Get<DbConfig>();
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
            return services;
        }

        public static bool CreateTables(DbConfig dbConfig)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
                x.GetTypes().Where(y => y.BaseType != null && y.BaseType.IsGenericType && y.BaseType.GetGenericTypeDefinition() == typeof(BaseEntity<,>)
                                        && y.FullName != null && !y.FullName.Contains("FreeSql")));
            BaseEntity.Orm.CodeFirst.SyncStructure(types.ToArray());
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), Configure.AppSettingsJsonPath);
            JObject jsonObject;
            using (StreamReader file = new StreamReader(filePath))
            {
                using JsonTextReader reader = new JsonTextReader(file);
                jsonObject = (JObject)JToken.ReadFrom(reader);
                jsonObject.Add("Db", JObject.FromObject(dbConfig));
            }

            using var writer = new StreamWriter(filePath);
            using JsonTextWriter jsonWriter = new JsonTextWriter(writer) {Formatting = Formatting.Indented};
            jsonObject.WriteTo(jsonWriter);
            return true;
        }

        private static (bool isSuccess, string msg) SetupDb(IServiceCollection services, DbConfig dbConfig)
        {
            if (!dbConfig.DbType.IsNullOrEmpty() && Enum.TryParse(dbConfig.DbType, true, out DataType dataType))
            {
                IFreeSql freeSql = null;
                var isDevelopment = services.GetSingletonInstanceOrNull<IHostEnvironment>()?.IsDevelopment() ?? true;
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
                            .UseConnectionString(dataType, $"data source={dbConfig.DbName}")
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