using Application.Commands;
using Data;
using Utils;
using WorkDispatcher;
using Microsoft.Extensions.DependencyInjection;

using P2PBank.Presentation.Interfaces;
using P2PBank.Presentation.Tcp;

public static class Program
{
    public static async Task Main(string[] args)
    {

        if(args.Length < 2)
        {
            Console.WriteLine("p2p_bank [config directory path] [log file path]");
            Console.WriteLine("p2p_bank default [ServerConfig/DatabaseCredentials]");
            return;
        }

        if(args[0] == "default")
        {
            switch(args[1])
            {
                case "DatabaseCredentials":
                    Console.WriteLine(new DatabaseCredentials().ToJson());
                    break;
                case "ServerConfig":
                    Console.WriteLine(new TcpServerConfig().ToJson());
                    break;
                default:
                    Console.WriteLine("invalid configuration");
                    break;
            }
            return;
        }

        string configDir = args[0];
        string logPath = args[1];

        Log.Info($"config directory: {configDir}");
        Log.Info($"log file: {logPath}");

        Log.Instance.FilePath = logPath;

        string dbConfig = Path.Combine(configDir, "DatabaseCredentials.json");
        string serverConfig = Path.Combine(configDir, "ServerConfig.json");
        
        ServiceCollection services = new();

        // presentation tcp
        services.AddSingleton<IServer, TcpServer>();
        services.AddScoped<ISession, TcpSession>();

        // application
        services.AddSingleton<Dispatcher<Command>>();
        var provider = services.();

        if(!File.Exists(dbConfig))
        {
            Log.Error($"{dbConfig} is required but it does not exist");
            return;
        }

        DatabaseCredentials credentials = DatabaseCredentials.FromJson(File.ReadAllText(dbConfig));

        Log.Info($"DatabaseCredentials loaded from {dbConfig}");
        Log.Info($"DatabaseCredentials: {credentials}");

        if(!Database.Exists(credentials))
        {
            Log.Info($"datbase does not exist CREATING");
            try
            {
                Database.Create(credentials);
            }
            catch
            {
                Log.Error($"failed to create database, check you DatabaseCredentials config");
                return;
            }

            Log.Info($"database created");
        }

        Database database;
        try
        {
            database = new Database(credentials);
        }
        catch
        {
            Log.Error($"failed to connect to database, check you DatabaseCredentials config");
            return;
        }

        TcpServerConfig config;
        if(!File.Exists(serverConfig))
        {
            Log.Info($"ServerConfig not found, using default options");
            config = new TcpServerConfig
            {
                Port = 5000,
                ClientTimeout = 30
            };
        }
        else
        {
            config = TcpServerConfig.FromJson(File.ReadAllText(serverConfig));
            Log.Info($"ServerConfig loaded from {serverConfig}");
        }
        Log.Info($"ServerConfig: {config}");

        TcpServer server = new(dispatcher, database, config);
        Log.Info($"starting server on {UtilFuncs.GetLocalIPAddress()}:{config.Port}");
        await server.StartAsync();

        database.Dispose();
    }
}