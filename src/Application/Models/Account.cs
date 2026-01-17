using Utils;

namespace Application.Models;

public class Account
{
    public int Id {get; set;}

    private long balance;
    public long Balance {
        set
        {
            if(value < 0)
            {
                throw new BankException("account balance cannot be negative");
            }
            balance = value;
        }
        get
        {
            return balance;
        }
    }
    public string? Ip {get; set;}

    public override string ToString()
    {
        return $"{Id}/{Ip}";
    }
}