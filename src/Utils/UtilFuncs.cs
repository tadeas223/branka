namespace P2PBank.Utils;

using System.Net;
using System.Net.Sockets;

public static class UtilFuncs
{    
    private static string? ip = null;
    public static string GetLocalIPAddress()
    {
        if(ip == null)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var address in host.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(address))
                {
                    ip = address.ToString();
                    return ip;
                }
            }
            throw new Exception("cound not get ip");
        }
        return ip;
    }

    public static (int, string)? ParseAccountStr(string accountStr)
    {
        string[] split = accountStr.Split("/");
        if(split.Length != 2)
        {
            return null;
        }

        if (!int.TryParse(split[0], out int id))
        {
            return null;
        }
        
        string ip = split[1];
        
        // Validace IP adresy
        if (!IPAddress.TryParse(ip, out _))
        {
            return null;
        }
        
        // Validace čísla účtu
        if (id < 10000 || id > 99999)
        {
            return null;
        }

        return (id, ip);
    }

}