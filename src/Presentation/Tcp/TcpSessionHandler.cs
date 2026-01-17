namespace P2PBank.Presentation.Tcp;

using System.Net.Sockets;
using System.Text;
using Utils;

public class TcpSessionHandler : IDisposable
{
    private MemoryStream buffer;
    private TcpSession session;
    private TcpServer server;
    private NetworkStream stream;

    private Task? task;
    private CancellationTokenSource cts = new();

    public event Action<string>? OnMessageReceived;

    public int Timeout {get; set;} = 5;

    public TcpSessionHandler(TcpServer server, TcpSession session)
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
        var bufferTemp = new byte[512];

        while (!cts.Token.IsCancellationRequested)
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(Timeout));
            using var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(cts.Token, timeoutCts.Token);

            int bytesRead;
            try
            {
                bytesRead = await stream.ReadAsync(bufferTemp, linkedCts.Token);
            }
            catch (OperationCanceledException)
            {
                server.TerminateSession(session, "read timeout");
                break;
            }
            catch (IOException)
            {
                server.TerminateSession(session, "connection closed");
                break;
            }

            if (bytesRead == 0)
            {
                server.TerminateSession(session, "connection closed");
                break;
            }

            buffer.Write(bufferTemp, 0, bytesRead);
            ProcessBuffer();
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
