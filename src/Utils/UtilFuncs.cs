using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Utils;

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

    public static (int, string) ParseAccountStr(string accountStr)
    {
        string[] split = accountStr.Split("/");
        if(split.Length != 2)
        {
            throw new BankException($"invalid account string {accountStr}");
        }

        int id = int.Parse(split[0]);
        string ip = split[1];

        return (id, ip);
    }

}