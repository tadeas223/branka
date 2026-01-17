using System.Data.Common;
using System.Net.Sockets;
using Microsoft.VisualBasic;

namespace Application;

public class BankConnection
{
    public static int PORT = 5000;
    private string ip;
    
    public BankConnection(string ip)
    {
        this.ip = ip;
    }

    public string BC()
    {
        using TcpClient client = new TcpClient(ip, PORT);
        using var writer = new StreamWriter(client.GetStream());
        using var reader = new StreamReader(client.GetStream());

        writer.WriteLine("BC");
        writer.Flush();

        string? line = reader.ReadLine();
        if(line == null)
        {
            throw new IOException("failed to read response");
        }


        string[] split = line.Split(" ");
        if(split[0] == "ER")
        {
            throw new Exception(split[1]);
        }

        return split[1];
    }
    

    public void AD(int id, long deposit)
    {
        using TcpClient client = new TcpClient(ip, PORT);
        using var writer = new StreamWriter(client.GetStream());
        using var reader = new StreamReader(client.GetStream());

        writer.WriteLine($"AD {id}/{ip} {deposit}");
        writer.Flush();

        string? line = reader.ReadLine();
        if(line == null)
        {
            throw new IOException("failed to read response");
        } 

        string[] split = line.Split(" ");
        if(split[0] == "ER")
        {
            throw new Exception(split[1]);
        }
    }
    
    public void AW(int id, long withdraw)
    {
        using TcpClient client = new TcpClient(ip, PORT);
        using var writer = new StreamWriter(client.GetStream());
        using var reader = new StreamReader(client.GetStream());

        writer.WriteLine($"AW {id}/{ip} {withdraw}");
        writer.Flush();

        string? line = reader.ReadLine();
        if(line == null)
        {
            throw new IOException("failed to read response");
        } 

        string[] split = line.Split(" ");
        if(split[0] == "ER")
        {
            throw new Exception(split[1]);
        }
    }
    
    public long AB(int id)
    {
        using TcpClient client = new TcpClient(ip, PORT);
        using var writer = new StreamWriter(client.GetStream());
        using var reader = new StreamReader(client.GetStream());

        writer.WriteLine($"AB {id}/{ip}");
        writer.Flush();

        string? line = reader.ReadLine();
        if(line == null)
        {
            throw new IOException("Failed to read response.");
        } 

        string[] split = line.Split(" ");
        if(split[0] == "ER")
        {
            throw new Exception(split[1]);
        }

        return long.Parse(split[1]);
    }
    
    public void AD(int id)
    {
        using TcpClient client = new TcpClient(ip, PORT);
        using var writer = new StreamWriter(client.GetStream());
        using var reader = new StreamReader(client.GetStream());

        writer.WriteLine($"AD {id}/{ip}");
        writer.Flush();

        string? line = reader.ReadLine();
        if(line == null)
        {
            throw new IOException("Failed to read response.");
        } 

        string[] split = line.Split(" ");
        if(split[0] == "ER")
        {
            throw new Exception(split[1]);
        }
    }
}