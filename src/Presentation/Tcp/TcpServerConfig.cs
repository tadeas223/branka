namespace P2PBank.Presentation.Tcp;

using System.Text.Json;
using P2PBank.Presentation.Interfaces;

public class TcpServerConfig : IServerConfig
{
    public int ClientTimeout {get; set;}
    public int Port {get; set;}

    public TcpServerConfig()
    {
        Port = 65525;
        ClientTimeout = 5;
    }

    public static TcpServerConfig FromJson(string json)
    {
        TcpServerConfig? result = JsonSerializer.Deserialize<TcpServerConfig>(json);
        if(result == null) throw new JsonException("Failed to parse credentials");
        return result;
    }
    
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public override string ToString()
    {
        return $"ServerConfig{{Port={Port}, ClientTimeout={ClientTimeout}}}";
    }
}