using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Utils;

namespace Presentation;

public class TcpSession : IDisposable
{
    public Socket Socket {get; private set;}
    public TcpServer Server {get; private set;}
    public Guid SessionId {get; private set;}
    
    public bool Connected
    {
        get
        {
            bool poll = !(Socket.Poll(0, SelectMode.SelectRead) && Socket.Available == 0);
            return poll;
        }
    }

    public TcpSession(TcpServer server, Socket socket)
    {
        Server = server;
        Socket = socket;
        SessionId = Guid.NewGuid();
    }
    
    public void Write(string msg)
    {
        using NetworkStream stream = new NetworkStream(Socket, ownsSocket: false);
        
        byte[] buffer = Encoding.UTF8.GetBytes(msg);
        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();
    }
    
    public void WriteLine(string msg)
    {
        msg += '\n';
        Write(msg);
    }
    
    public async Task WriteAsync(string msg)
    {
        using NetworkStream stream = new NetworkStream(Socket, ownsSocket: false);
        
        byte[] buffer = Encoding.UTF8.GetBytes(msg);
        await stream.WriteAsync(buffer, 0, buffer.Length);
        stream.Flush();
    }
    
    public async Task WriteLineAsync(string msg)
    {
        msg += '\n';
        await WriteAsync(msg);
    }


    public void Register()
    {
        Server.RegisterSession(this);
    }

    public void Dispose()
    {
        Server.TerminateSession(this, "unknown");
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