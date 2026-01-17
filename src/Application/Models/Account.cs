namespace Application.Models;

public class Account
{
    public int Id {get; set;}
    public long Balace {get; set;}
    public string? Ip {get; set;}

    public Account(int id, string ip, long balance)
    {
        Id = id;
        Ip = ip;
        Balace = balance;
    }
    
    public Account(int id, string ip)
    {
        Id = id;
        Ip = ip;
        Balace = 0;
    }

}