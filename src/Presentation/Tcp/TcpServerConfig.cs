namespace P2PBank.Presentation.Tcp;

using System.Text.Json;
using P2PBank.Presentation.Interface;
using P2PBank.Utils;

public class TcpServerConfig : ServerConfig
{
    public int ClientTimeout {get; set;}
    public int Port {get; set;}

    public override Config DefaultConfig
    {
        get
        {
            return new TcpServerConfig
            {
                ClientTimeout = 5,
                Port = 65525
            };

        }
    }
    
    public static TcpServerConfig Deserialize(string json)
    {
        TcpServerConfig? result = JsonSerializer.Deserialize<TcpServerConfig>(json);
        if(result == null) throw new JsonException("Failed to parse credentials");
        return result;
    }
    
    public override string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }
    
    public override string ToString()
    {
        return $"ServerConfig{{Port={Port}, ClientTimeout={ClientTimeout}}}";
    }
}