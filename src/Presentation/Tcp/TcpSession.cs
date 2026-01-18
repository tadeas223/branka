namespace P2PBank.Presentation.Tcp;

using System.Net.Sockets;
using System.Text;
using P2PBank.Presentation.Interface;
using P2PBank.Utils;

public class TcpSession : ISession
{
    public Socket Socket {get; private set;}
    public Guid SessionId {get; private set;}
    public ManualResetEvent ProcessingRequest {get;}
    
    public bool Connected
    {
        get
        {
            bool poll = !(Socket.Poll(0, SelectMode.SelectRead) && Socket.Available == 0);
            return poll;
        }
    }

    public string HostIdentifier
    {
        get
        {
            if(Socket.RemoteEndPoint == null)
            {
                return "";
            }
            return Socket.RemoteEndPoint!.ToString() ?? "";
        }
    }

    private Log log;

    public TcpSession(Socket socket, Log log)
    {
        Socket = socket;
        SessionId = Guid.NewGuid();
        this.log = log;
        ProcessingRequest = new(false);
    }
    
    public void Write(string msg)
    {
        using NetworkStream stream = new NetworkStream(Socket, ownsSocket: false);
        
        byte[] buffer = Encoding.UTF8.GetBytes(msg);
        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();
        log.Info($"{HostIdentifier} {msg}");
    }
    
    public void WriteLine(string msg)
    {
        msg += '\n';
        Write(msg);
        log.Info($"{HostIdentifier} {msg}");
    }
    
    public async Task WriteAsync(string msg)
    {
        using NetworkStream stream = new NetworkStream(Socket, ownsSocket: false);
        
        byte[] buffer = Encoding.UTF8.GetBytes(msg);
        await stream.WriteAsync(buffer, 0, buffer.Length);
        stream.Flush();
        log.Info($"{HostIdentifier} {msg}");
    }
    
    public async Task WriteLineAsync(string msg)
    {
        msg += '\n';
        log.Info($"{HostIdentifier} {msg}");
        await WriteAsync(msg);
    }
    
    public void WriteDontLog(string msg)
    {
        using NetworkStream stream = new NetworkStream(Socket, ownsSocket: false);
        
        byte[] buffer = Encoding.UTF8.GetBytes(msg);
        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();
    }
    
    public void WriteLineDontLog(string msg)
    {
        msg += '\n';
        Write(msg);
    }
    
    public async Task WriteDontLogAsync(string msg)
    {
        using NetworkStream stream = new NetworkStream(Socket, ownsSocket: false);
        
        byte[] buffer = Encoding.UTF8.GetBytes(msg);
        await stream.WriteAsync(buffer, 0, buffer.Length);
        stream.Flush();
    }
    
    public async Task WriteLineDontLogAsync(string msg)
    {
        msg += '\n';
        await WriteAsync(msg);
    }

    public void Dispose()
    {
        Socket.Close();
        Socket.Dispose();
    }

    public override bool Equals(object? obj)
    {
        if(obj is TcpSession other)
        {
            return SessionId.Equals(other.SessionId);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return SessionId.GetHashCode();
    }
}