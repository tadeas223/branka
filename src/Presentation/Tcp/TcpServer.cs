namespace P2PBank.Presentation.Tcp;

using Data;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Application;
using Application.Commands;
using Utils;
using WorkDispatcher;
using P2PBank.Presentation.Interfaces;

public class TcpServer : IServer
{
    private TcpServerConfig config;
    private CancellationTokenSource cts = new();
    private Dispatcher<Command> commandDispatcher;

    ConcurrentDictionary<TcpSession, TcpSessionHandler> sessions = new();

    public TcpServer(Dispatcher<Command> commandDispatcher, IServerConfig? config = null)
    {
        this.commandDispatcher = commandDispatcher;

        if(config == null)
        {
            this.config = new TcpServerConfig();
        }

        TcpServerConfig? tcpConfig = config as TcpServerConfig;
        if(tcpConfig == null)
        {
            Log.Error("TcpServer got incompatable configuration, setting to default");
        }

        this.config = tcpConfig!;
    }
    
    public void Start()
    {
        using TcpListener socketListener = new(IPAddress.Any, config.Port);
        socketListener.Start();
        while(!cts.Token.IsCancellationRequested)
        {
            Socket socket = socketListener.AcceptSocket();
            
            TcpSession session = new TcpSession(socket);
            RegisterSession(session);
        }
        socketListener.Stop();
    }

    public async Task StartAsync()
    {
        using TcpListener socketListener = new(IPAddress.Any, config.Port);
        socketListener.Start();
        while(!cts.Token.IsCancellationRequested)
        {
            Socket socket = await socketListener.AcceptSocketAsync();
            
            TcpSession session = new TcpSession(socket);
            RegisterSession(session);
        }
        socketListener.Stop();
    }

    public void TerminateSession(ISession session, string reason)
    {
        TcpSession? tcpSession = session as TcpSession;
        if(tcpSession == null)
        {
            Log.Warn($"attempted to terminate an invalid session class, session was propably LEAKED");
            return;
        }

        TcpSessionHandler? sessionHandler;
        sessions.Remove(tcpSession, out sessionHandler);
        if(sessionHandler == null)
        {
            Log.Warn($"session {tcpSession.Socket.RemoteEndPoint} was not registered with this server, session handler is LEAKED");
            return;
        }

        sessionHandler.Dispose();

        Log.Info($"session {tcpSession.Socket.RemoteEndPoint}: terminated reason {reason}");
    }

    public void RegisterSession(ISession session)
    {
        TcpSession? tcpSession = session as TcpSession;
        if(tcpSession == null)
        {
            Log.Warn($"attempted to register an invalid session class, is not registered");
            return;
        }

        TcpSessionHandler handler = new TcpSessionHandler(this, tcpSession)
        {
            Timeout = config.ClientTimeout
        };
        sessions[tcpSession] = handler;

        handler.OnMessageReceived += (msg) => 
        {
            Command cmd = CommandParser.Parse(msg);
            cmd.Session = tcpSession;
            commandDispatcher.Add(cmd);
        };

        handler.Start();

        Log.Info($"session {tcpSession.Socket.RemoteEndPoint} registered");
    }

    public void Dispose()
    {
        foreach(var kvp in sessions)
        {
            kvp.Value.Dispose();
        }
        cts.Cancel();
        cts.Dispose();
    }    
}