using System.Net.Sockets;
using System.Text;
using Presentation;
using Utils;
using System.Diagnostics;

public class SessionHandler : IDisposable
{
    private MemoryStream buffer;
    private TcpSession session;
    private TcpServer server;
    private NetworkStream stream;

    private Task? task;
    private CancellationTokenSource cts = new();

    public event Action<string>? OnMessageReceived;

    public int Timeout {get; set;} = 5;

    public SessionHandler(TcpServer server, TcpSession session)
    {
        this.server = server;
        this.session = session;
        stream = new NetworkStream(session.Socket, ownsSocket: false);
        buffer = new MemoryStream();
    }

    public void Start()
    {
        task = StartAsync();
    }

    private async Task StartAsync()
    {
        byte[] temp = new byte[512];

        Stopwatch stopwatch = new Stopwatch();
        
        while (!cts.Token.IsCancellationRequested)
        {
            stopwatch.Start();

            if(!session.Connected)
            {
                server.TerminateSession(session);
                break;
            }

            Task<int> readTask = stream.ReadAsync(temp, 0, temp.Length);
            Task completedTask = await Task.WhenAny(readTask, Task.Delay(Timeout * 1000));

            if(completedTask != readTask)
            {
                server.TerminateSession(session);
                break;
            }

            int bytesRead = await readTask;

            if (bytesRead == 0)
            {
                server.TerminateSession(session);
                break;
            }

            buffer.Write(temp, 0, bytesRead);

            ProcessBuffer();

            stopwatch.Reset();
        }
    }

    private void ProcessBuffer()
    {
        buffer.Position = 0;
        using var reader = new StreamReader(buffer, Encoding.UTF8, false, 1024, true);

        string? line;
        int lastPos = 0;

        while ((line = reader.ReadLine()) != null)
        {
            lastPos = (int)reader.BaseStream.Position;
            OnMessageReceived?.Invoke(line);
            Log.Info($"{session.Socket.RemoteEndPoint} sent: {line}");
        }

        byte[] remaining = buffer.ToArray()[lastPos..];
        buffer.SetLength(0);
        buffer.Write(remaining, 0, remaining.Length);
    }

    public void Dispose()
    {
        cts.Cancel();

        stream.Dispose();
        buffer.Dispose();
        cts.Dispose();
    }
}
