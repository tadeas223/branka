namespace P2PBank.Application;

using System.Net.Sockets;

public class BankConnection
{
    private string ip;
    private int port;
    private int timeoutSeconds;
    
    public BankConnection(string ip, int port, int timeoutSeconds = 5)
    {
        this.ip = ip;
        this.port = port;
        this.timeoutSeconds = timeoutSeconds;
    }

    public string BC()
    {
        using TcpClient client = new TcpClient();
        if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(timeoutSeconds)))
        {
            throw new IOException("Connection timeout");
        }
        
        using var stream = client.GetStream();
        stream.ReadTimeout = timeoutSeconds * 1000;
        stream.WriteTimeout = timeoutSeconds * 1000;
        
        using var writer = new StreamWriter(stream);
        using var reader = new StreamReader(stream);

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
        using TcpClient client = new TcpClient();
        if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(timeoutSeconds)))
        {
            throw new IOException("Connection timeout");
        }
        
        using var stream = client.GetStream();
        stream.ReadTimeout = timeoutSeconds * 1000;
        stream.WriteTimeout = timeoutSeconds * 1000;
        
        using var writer = new StreamWriter(stream);
        using var reader = new StreamReader(stream);

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
        using TcpClient client = new TcpClient();
        if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(timeoutSeconds)))
        {
            throw new IOException("Connection timeout");
        }
        
        using var stream = client.GetStream();
        stream.ReadTimeout = timeoutSeconds * 1000;
        stream.WriteTimeout = timeoutSeconds * 1000;
        
        using var writer = new StreamWriter(stream);
        using var reader = new StreamReader(stream);

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
        using TcpClient client = new TcpClient();
        if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(timeoutSeconds)))
        {
            throw new IOException("Connection timeout");
        }
        
        using var stream = client.GetStream();
        stream.ReadTimeout = timeoutSeconds * 1000;
        stream.WriteTimeout = timeoutSeconds * 1000;
        
        using var writer = new StreamWriter(stream);
        using var reader = new StreamReader(stream);

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
    
    public void AR(int id)
    {
        using TcpClient client = new TcpClient();
        if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(timeoutSeconds)))
        {
            throw new IOException("Connection timeout");
        }
        
        using var stream = client.GetStream();
        stream.ReadTimeout = timeoutSeconds * 1000;
        stream.WriteTimeout = timeoutSeconds * 1000;
        
        using var writer = new StreamWriter(stream);
        using var reader = new StreamReader(stream);

        writer.WriteLine($"AR {id}/{ip}");
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