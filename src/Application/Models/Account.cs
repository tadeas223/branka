using Utils;

namespace Application.Models;

public class Account
{
    public int Id {get; set;}
    public long Balance {
        set
        {
            if(value < 0)
            {
                throw new BankException("account balance cannot be negative");
            }
            Balance = value;
        }
        get
        {
            return Balance;
        }
    }
    public string? Ip {get; set;}

    public override string ToString()
    {
        return $"{Id}/{Ip}";
    }
}