using System.ComponentModel;
using System.Numerics;

namespace Presentation;

public struct TcpServerConfig
{
    public int ClientTimeout {get; set;}
    public int Port {get; set;}

    public TcpServerConfig()
    {
        Port = 65525;
        ClientTimeout = 5;
    }
}