using System.Net;
using System.Net.Sockets;
using Presentation;

namespace Application.Commands;

public class BrCommand : Command
{

    public override void Execute()
    {
        string? ip = GetLocalIPAddress();
        if(ip == null)
        {
            Session.WriteLine($"ER internal error while reteiving an ip address");
        }
        Session.WriteLine($"BR {ip}");
    }
    
    private static string? GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
            {
                return ip.ToString();
            }
        }
        return null;
    }
}