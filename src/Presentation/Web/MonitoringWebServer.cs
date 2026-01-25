namespace P2PBank.Presentation.Web;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using P2PBank.Presentation.Interface;
using P2PBank.Data.Interface;
using P2PBank.Utils;
using P2PBank.Presentation.Tcp;

public class MonitoringWebServer
{
    private WebApplication? app;
    private IServer? tcpServer;
    private IAccountRepository? accountRepository;
    private Log? log;
    private ServerConfig? serverConfig;
    private CancellationTokenSource? shutdownCts;

    public MonitoringWebServer(IServer tcpServer, IAccountRepository accountRepository, Log log, ServerConfig serverConfig)
    {
        this.tcpServer = tcpServer;
        this.accountRepository = accountRepository;
        this.log = log;
        this.serverConfig = serverConfig;
        this.shutdownCts = new CancellationTokenSource();
    }

    public void Start(int port = 8080)
    {
        var builder = WebApplication.CreateBuilder();
        app = builder.Build();
        app.Urls.Add($"http://localhost:{port}");

        // Serve static HTML
        app.MapGet("/", async (HttpContext context) =>
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(GetHtmlContent());
        });

        // API endpoints
        app.MapGet("/api/status", async (HttpContext context) =>
        {
            var status = GetStatus();
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(status));
        });

        app.MapGet("/api/sessions", async (HttpContext context) =>
        {
            var sessions = GetSessions();
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(sessions));
        });

        app.MapGet("/api/accounts", async (HttpContext context) =>
        {
            var accounts = GetAccounts();
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(accounts));
        });

        app.MapPost("/api/shutdown", async (HttpContext context) =>
        {
            log?.Info("Shutdown requested via web interface");
            shutdownCts?.Cancel();
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("{\"status\":\"shutting down\"}");
        });

        _ = Task.Run(async () =>
        {
            try
            {
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                log?.Error($"Web server error: {ex}");
            }
        });
        log?.Info($"Monitoring web interface started on http://localhost:{port}");
    }

    public CancellationToken GetShutdownToken()
    {
        return shutdownCts?.Token ?? CancellationToken.None;
    }

    private object GetStatus()
    {
        TcpServerConfig? tcpConfig = serverConfig as TcpServerConfig;
        string localIp = UtilFuncs.GetLocalIPAddress();
        
        long totalBalance = 0;
        int clientCount = 0;
        try
        {
            totalBalance = accountRepository?.TotalBalance ?? 0;
            clientCount = accountRepository?.ClientCount ?? 0;
        }
        catch { }

        int activeSessions = tcpServer?.GetActiveSessions().Count() ?? 0;

        return new
        {
            bankCode = localIp,
            port = tcpConfig?.Port ?? 65525,
            totalBalance = totalBalance,
            clientCount = clientCount,
            activeSessions = activeSessions,
            status = "running"
        };
    }

    private object GetSessions()
    {
        var sessions = tcpServer?.GetActiveSessions().Select(s => new
        {
            sessionId = s.SessionId.ToString(),
            hostIdentifier = s.HostIdentifier,
            connected = s.Connected
        }) ?? Enumerable.Empty<object>();

        return new { sessions = sessions };
    }

    private object GetAccounts()
    {
        // Note: This is a simplified version. In a real implementation,
        // you might want to add a method to IAccountRepository to get all accounts
        return new { message = "Account list not implemented in repository" };
    }

    private string GetHtmlContent()
    {
        return @"<!DOCTYPE html>
<html lang=""cs"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>P2P Bank Monitoring</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            padding: 20px;
        }
        .container {
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            border-radius: 10px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.2);
            padding: 30px;
        }
        h1 {
            color: #333;
            margin-bottom: 30px;
            text-align: center;
        }
        .status-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        .status-card {
            background: #f8f9fa;
            border-left: 4px solid #667eea;
            padding: 20px;
            border-radius: 5px;
        }
        .status-card h3 {
            color: #667eea;
            margin-bottom: 10px;
            font-size: 14px;
            text-transform: uppercase;
        }
        .status-card .value {
            font-size: 32px;
            font-weight: bold;
            color: #333;
        }
        .sessions {
            margin-top: 30px;
        }
        .sessions h2 {
            margin-bottom: 15px;
            color: #333;
        }
        .session-item {
            background: #f8f9fa;
            padding: 15px;
            margin-bottom: 10px;
            border-radius: 5px;
            border-left: 4px solid #28a745;
        }
        .shutdown-btn {
            background: #dc3545;
            color: white;
            border: none;
            padding: 15px 30px;
            font-size: 16px;
            border-radius: 5px;
            cursor: pointer;
            margin-top: 20px;
            width: 100%;
            transition: background 0.3s;
        }
        .shutdown-btn:hover {
            background: #c82333;
        }
        .refresh-btn {
            background: #28a745;
            color: white;
            border: none;
            padding: 10px 20px;
            font-size: 14px;
            border-radius: 5px;
            cursor: pointer;
            margin-bottom: 20px;
        }
        .refresh-btn:hover {
            background: #218838;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <h1>🏦 P2P Bank Node Monitoring</h1>
        
        <button class=""refresh-btn"" onclick=""refreshData()"">🔄 Aktualizovat</button>
        
        <div class=""status-grid"" id=""statusGrid"">
            <div class=""status-card"">
                <h3>Kód banky</h3>
                <div class=""value"" id=""bankCode"">-</div>
            </div>
            <div class=""status-card"">
                <h3>Port</h3>
                <div class=""value"" id=""port"">-</div>
            </div>
            <div class=""status-card"">
                <h3>Celkový zůstatek</h3>
                <div class=""value"" id=""totalBalance"">-</div>
            </div>
            <div class=""status-card"">
                <h3>Počet klientů</h3>
                <div class=""value"" id=""clientCount"">-</div>
            </div>
            <div class=""status-card"">
                <h3>Aktivní session</h3>
                <div class=""value"" id=""activeSessions"">-</div>
            </div>
        </div>

        <div class=""sessions"">
            <h2>Aktivní TCP Session</h2>
            <div id=""sessionsList"">Načítání...</div>
        </div>

        <button class=""shutdown-btn"" onclick=""shutdown()"">🛑 Bezpečně vypnout server</button>
    </div>

    <script>
        function formatNumber(num) {
            return new Intl.NumberFormat('cs-CZ').format(num);
        }

        async function refreshData() {
            try {
                const statusResponse = await fetch('/api/status');
                const status = await statusResponse.json();
                
                document.getElementById('bankCode').textContent = status.bankCode || '-';
                document.getElementById('port').textContent = status.port || '-';
                document.getElementById('totalBalance').textContent = formatNumber(status.totalBalance) + ' $';
                document.getElementById('clientCount').textContent = status.clientCount || '0';
                document.getElementById('activeSessions').textContent = status.activeSessions || '0';

                const sessionsResponse = await fetch('/api/sessions');
                const sessionsData = await sessionsResponse.json();
                
                const sessionsList = document.getElementById('sessionsList');
                if (sessionsData.sessions && sessionsData.sessions.length > 0) {
                    sessionsList.innerHTML = sessionsData.sessions.map(s => 
                        `<div class=""session-item"">
                            <strong>Session ID:</strong> ${s.sessionId.substring(0, 8)}...<br>
                            <strong>Host:</strong> ${s.hostIdentifier}<br>
                            <strong>Status:</strong> ${s.connected ? '✅ Připojeno' : '❌ Odpojeno'}
                        </div>`
                    ).join('');
                } else {
                    sessionsList.innerHTML = '<div class=""session-item"">Žádné aktivní session</div>';
                }
            } catch (error) {
                console.error('Chyba při načítání dat:', error);
            }
        }

        async function shutdown() {
            if (confirm('Opravdu chcete vypnout server?')) {
                try {
                    await fetch('/api/shutdown', { method: 'POST' });
                    alert('Server se vypíná...');
                } catch (error) {
                    console.error('Chyba při vypínání:', error);
                }
            }
        }

        // Auto-refresh každých 2 sekundy
        refreshData();
        setInterval(refreshData, 2000);
    </script>
</body>
</html>";
    }
}
