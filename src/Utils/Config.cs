namespace P2PBank.Utils;

public abstract class Config
{
    public abstract Config DefaultConfig {get;}

    public abstract string Serialize();    
}