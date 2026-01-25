namespace P2PBank.Presentation.Tcp;

using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

using WorkDispatcher;

using P2PBank.Presentation.Interface;
using P2PBank.Application.Interface;
using P2PBank.Utils;

public class TcpServer : IServer
{
    private TcpServerConfig config;
    private CancellationTokenSource cts = new();
    private Dispatcher<Command> commandDispatcher;
    private Log log;
    private ICommandFactory commandFactory;

    ConcurrentDictionary<TcpSession, TcpSessionHandler> sessions = new();

    public TcpServer(Dispatcher<Command> commandDispatcher, Log log, ICommandFactory commandFactory, ServerConfig? config = null)
    {
        this.commandDispatcher = commandDispatcher;
        this.commandFactory = commandFactory;

        if(config == null)
        {
            this.config = (new TcpServerConfig().DefaultConfig as TcpServerConfig)!;
        }

        TcpServerConfig? tcpConfig = config as TcpServerConfig;
        if(tcpConfig == null)
        {
            log.Error("TcpServer got incompatable configuration, setting to default");
        }

        this.config = tcpConfig!;
        this.log = log;
    }
    
    public async Task StartAsync()
    {
        using TcpListener socketListener = new(IPAddress.Any, config.Port);
        socketListener.Start(100_000);
        while(!cts.Token.IsCancellationRequested)
        {
            Socket socket = await socketListener.AcceptSocketAsync();
            
            TcpSession session = new TcpSession(socket, log);
            RegisterSession(session);
        }
        socketListener.Stop();
    }

    public void TerminateSession(ISession session, string reason)
    {
        TcpSession? tcpSession = session as TcpSession;
        if(tcpSession == null)
        {
            log.Warn($"attempted to terminate an invalid session class, session was propably LEAKED");
            return;
        }

        TcpSessionHandler? sessionHandler;
        sessions.Remove(tcpSession, out sessionHandler);
        if(sessionHandler == null)
        {
            log.Warn($"session {tcpSession.HostIdentifier} was not registered with this server, session handler is LEAKED");
            return;
        }

        sessionHandler.Dispose();

        log.Info($"session {tcpSession.HostIdentifier}: terminated reason {reason}");
    }

    public void RegisterSession(ISession session)
    {
        TcpSession? tcpSession = session as TcpSession;
        if(tcpSession == null)
        {
            log.Warn($"attempted to register an invalid session class, is not registered");
            return;
        }

        TcpSessionHandler handler = new TcpSessionHandler(this, tcpSession, log)
        {
            Timeout = config.ClientTimeout
        };
        sessions[tcpSession] = handler;

        handler.OnMessageReceived += (msg) => 
        {
            _ = Task.Run(() =>
            {
                try
                {
                    Command cmd = commandFactory.Create(msg);
                    cmd.Session = tcpSession;
                    commandDispatcher.Add(cmd);
                }
                catch (Exception ex)
                {
                    log.Error($"Command processing failed: {ex}");
                }
            });
        };

        handler.Start();

        log.Info($"session {tcpSession.HostIdentifier} registered");
    }

    public IEnumerable<ISession> GetActiveSessions()
    {
        return sessions.Keys.Cast<ISession>().Where(s => s.Connected).ToList();
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
