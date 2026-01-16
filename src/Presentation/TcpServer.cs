using System.Collections.Concurrent;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Intrinsics.X86;
using Application;
using Application.Commands;
using Utils;

namespace Presentation;

public class TcpServer : IDisposable
{
    public static readonly int PORT=5000;

    CancellationTokenSource cts = new();
    TcpListener socketListener = new TcpListener(IPAddress.Any, PORT);
    CommandDispatcher dispatcher;

    ConcurrentDictionary<TcpSession, SessionHandler> sessions = new();

    public TcpServer(CommandDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
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

    public void TerminateSession(TcpSession session)
    {
        SessionHandler? sessionHandler;
        sessions.Remove(session, out sessionHandler);
        if(sessionHandler == null)
        {
            Log.Warn($"session {session.Socket.RemoteEndPoint} was not registered with this server, session handler is LEAKED");
            session.Dispose();
            return;
        }

        sessionHandler.Dispose();

        Log.Info($"session terminated: {session.Socket.RemoteEndPoint}");
    }

    public void RegisterSession(TcpSession session)
    {
        SessionHandler handler = new SessionHandler(this, session);
        sessions[session] = handler;

        handler.OnMessageReceived += (msg) => 
        {
            Command cmd = CommandParser.Parse(msg);
            dispatcher.Enqueue(cmd);
        };

        handler.Start();

        Log.Info($"session registered: {session.Socket.RemoteEndPoint}");
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
