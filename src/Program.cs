using Application;
using Application.Commands;
using Presentation;
using Utils;
using WorkDispatcher;

Log.Instance.FilePath = "logs.txt";

using Dispatcher<Command> dispatcher = new(5);
dispatcher.Start();

TcpServer server = new(dispatcher);
await server.StartAsync();