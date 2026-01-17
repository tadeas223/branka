using Data;
using System.Collections.Concurrent;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Intrinsics.X86;
using Application;
using Application.Commands;
using Utils;
using WorkDispatcher;

namespace Presentation;

public class TcpServer : IDisposable
{
    public static readonly int PORT=5000;

    CancellationTokenSource cts = new();
    TcpListener socketListener = new TcpListener(IPAddress.Any, PORT);
    Dispatcher<Command> commandDispatcher;
    Database database;

    ConcurrentDictionary<TcpSession, SessionHandler> sessions = new();

    public TcpServer(Dispatcher<Command> commandDispatcher, Database database)
    {
        this.commandDispatcher = commandDispatcher;
        this.database = database;
    }

    public async Task StartAsync()
    {
        socketListener.Start();
        while(!cts.Token.IsCancellationRequested)
        {
            Socket socket = await socketListener.AcceptSocketAsync();
            
            TcpSession session = new TcpSession(this, socket);
            session.Register();
        }
        socketListener.Stop();
    }

    public void TerminateSession(TcpSession session, string reason)
    {
        SessionHandler? sessionHandler;
        sessions.Remove(session, out sessionHandler);
        if(sessionHandler == null)
        {
            Log.Warn($"session {session.Socket.RemoteEndPoint} was not registered with this server, session handler is LEAKED");
            return;
        }

        sessionHandler.Dispose();

        Log.Info($"session {session.Socket.RemoteEndPoint}: terminated reason {reason}");
    }

    public void RegisterSession(TcpSession session)
    {
        SessionHandler handler = new SessionHandler(this, session);
        sessions[session] = handler;

        handler.OnMessageReceived += (msg) => 
        {
            Command cmd = CommandParser.Parse(msg);
            cmd.Session = session;
            cmd.Database = database;
            commandDispatcher.Add(cmd);
        };

        handler.Start();

        Log.Info($"session {session.Socket.RemoteEndPoint} registered");
    }

    public void Dispose()
    {
        foreach(var kvp in sessions)
        {
            kvp.Value.Dispose();
        }
        cts.Cancel();
    }    

    private struct SessionHandlerTask
    {
        public SessionHandler handler;
        public Task task;

        public SessionHandlerTask(SessionHandler handler, Task task)
        {
            this.handler = handler;
            this.task = task;
        }
    }
}
