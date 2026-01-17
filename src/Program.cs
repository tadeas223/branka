using Application;
using Application.Commands;
using Data;
using Application.Models;
using Presentation;
using Utils;
using WorkDispatcher;

Log.Instance.FilePath = "logs.txt";

using Dispatcher<Command> dispatcher = new(5);
dispatcher.Start();

DatabaseCredentials credentials = DatabaseCredentials.FromJson(File.ReadAllText("database_config.json"));

if(!Database.Exists(credentials))
{
    Database.Create(credentials);
}

using Database database = new Database(credentials);

TcpServer server = new(dispatcher, database);
await server.StartAsync();