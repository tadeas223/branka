namespace P2PBank.Utils;

using System.Net;
using P2PBank.Application.Interface;

public static class Validators
{
    public static void ValidateAccountNumber(int accountNumber)
    {
        if (accountNumber < 10000 || accountNumber > 99999)
        {
            throw new UnifiedMessageException("Formát čísla účtu není správný.");
        }
    }
    
    public static void ValidateAmount(long amount)
    {
        if (amount < 0)
        {
            throw new UnifiedMessageException("Částka nemůže být záporná.");
        }
        if (amount > 9223372036854775807)
        {
            throw new UnifiedMessageException("Částka překračuje maximální povolenou hodnotu.");
        }
    }
    
    public static void ValidateIPAddress(string ip)
    {
        if (!IPAddress.TryParse(ip, out _))
        {
            throw new UnifiedMessageException("Formát IP adresy není správný.");
        }
    }
}
