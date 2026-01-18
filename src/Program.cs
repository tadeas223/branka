using P2PBank.Application.Commands;
using WorkDispatcher;
using Microsoft.Extensions.DependencyInjection;

using P2PBank.Data.Interface;
using P2PBank.Data.MicrosoftSql;

using P2PBank.Presentation.Interface;
using P2PBank.Presentation.Tcp;

using P2PBank.Application.Interface;

using P2PBank.Utils;
using System.Reflection;
public static class Program
{
    public static async Task Main(string[] args)
    {
        if(args.Length < 2)
        {
            Console.WriteLine("p2p_bank [config directory path] [log file path]");
            Console.WriteLine("p2p_bank default [ServerConfig/DatabaseConfig]");
            return;
        }

        string configDir = args[0];
        string logFile = args[1];
        
        string databaseConfigFile = Path.Combine(configDir, "DatabaseConfig.json");
        string serverConfigFile = Path.Combine(configDir, "ServerConfig.json");
        
        ServiceCollection services = new();

        // setup dependency injection
        Log log = new Log {FilePath = logFile };
        services.AddSingleton<Log>(sp => log);

        DiPresentation(log, services, (args[0] == "default")? null: serverConfigFile);
        DiApplication(log, services); 
        DiData(log, services, (args[0] == "default")? null: databaseConfigFile);

        using ServiceProvider provider = services.BuildServiceProvider();

        if(args[0] == "default")
        {
            Default(args, provider);
            return;
        }
        
        IServer? server = provider.GetService<IServer>();
        if(server == null)
        {
            log.Error("missing IServer service");
            return;
        }

        await server.StartAsync();
    }

    public static void Default(string[] args, ServiceProvider provider)
    {
        ServerConfig? serverConfig = provider.GetService<ServerConfig>();
        if(serverConfig == null)
        {
            throw new Exception("missing ServerConfig service");
        }
        
        DatabaseConfig? databaseConfig = provider.GetService<DatabaseConfig>();
        if(databaseConfig == null)
        {
            throw new Exception("missing DatabaseConfig service");
        }
        
        switch(args[1])
        {
            case "DatabaseConfig":
                Console.WriteLine(databaseConfig.DefaultConfig.Serialize());
                break;
            case "ServerConfig":
                Console.WriteLine(serverConfig.DefaultConfig.Serialize());
                break;
            default:
                Console.WriteLine("invalid configuration");
                break;
        }
    }

    public static void DiApplication(Log log, ServiceCollection services)
    {
        services.AddSingleton<Dispatcher<Command>>(sp => 
        {
            int workers = 10;
            log.Info($"using {workers} workers");
            var dispatcher = new Dispatcher<Command>(workers);
            dispatcher.Start();
            return dispatcher;
        });
        
        services.AddTransient<AbCommand>();
        services.AddTransient<AcCommand>();
        services.AddTransient<AdCommand>();
        services.AddTransient<ArCommand>();
        services.AddTransient<AwCommand>();
        services.AddTransient<BaCommand>();
        services.AddTransient<BcCommand>();
        services.AddTransient<BnCommand>();
        services.AddTransient<ErCommand>();
        
        services.AddTransient<ICommandFactory, CommandFactory>();
    }

    public static void DiPresentation(Log log, ServiceCollection services, string? serverConfigFile)
    {
        services.AddSingleton<IServer, TcpServer>();
        services.AddTransient<ISession, TcpSession>();
        services.AddSingleton<ServerConfig>(sp =>
        {
            if(serverConfigFile != null)
            {
                try
                {
                    TcpServerConfig config = TcpServerConfig.Deserialize(File.ReadAllText(serverConfigFile));
                    log.Info("ServerConfig config loaded");
                    return config;
                }
                catch
                {
                    log.Warn("failed to load ServerConfig, using default");
                    return (ServerConfig)new TcpServerConfig().DefaultConfig;
                }
            }
            return (ServerConfig)new TcpServerConfig().DefaultConfig;
        });
    }

    public static void DiData(Log log, ServiceCollection services, string? databaseConfigFile)
    {
        DatabaseConfig config;
        MicrosoftSqlDatabase database; 
        if(databaseConfigFile != null)
        {
            try
            {
                var mssqlConfig = MicrosoftSqlDatabaseConfig.Deserialize(File.ReadAllText(databaseConfigFile));
                log.Info("DatabaseConfig config loaded");
                config = mssqlConfig;
            }
            catch
            {
                log.Warn("failed to load DatabaseConfig, using default");
                config = (DatabaseConfig)new MicrosoftSqlDatabaseConfig().DefaultConfig;
            }

            database = new MicrosoftSqlDatabase(config);
            try
            {
                if(!database.Exists())
                {
                    database.Create();
                }
                database.Connect();
            }
            catch(Exception e)
            {
                log.Error($"failed to connect or create to the database {e}");
            }
        }
        else
        {
            config = (DatabaseConfig)new MicrosoftSqlDatabaseConfig().DefaultConfig;
            database = new MicrosoftSqlDatabase(config);
        }

        services.AddSingleton<MicrosoftSqlDatabase>(sp =>
        {
            return database;
        });
        
        services.AddSingleton<DatabaseConfig>(sp =>
        {
            return config;
        });
        
        services.AddTransient<IAccountRepository, MicrosoftSqlAccountRepository>();

        
    }
}
