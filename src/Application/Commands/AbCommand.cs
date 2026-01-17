using Data;
using Application.Models;
using Utils;

namespace Application.Commands;

public class AbCommand: Command
{
    public override void InternalExecute()
    {
        EnsusreParams(1);

        (int, string)? accountTuple = UtilFuncs.ParseAccountStr(Params[0]);
        if(!accountTuple.HasValue)
        {
            throw new UnifiedMessageException("Invalid account format.");
        }
        (int id, string ip) = accountTuple.Value;

        long balance = 0;

        if(ip == UtilFuncs.GetLocalIPAddress())
        {
            try
            {
                var accRepo = new AccountRepository(Database);
                Account acc = accRepo.SelectById(id);
                balance = acc.Balance;
            }
            catch
            {
                throw new UnifiedMessageException("Account was not found.");
            }
        }
        else
        {
            BankConnection con = new(ip);

            balance = con.AB(id);
        }

        Session.WriteLine($"AB {balance}");
    }
}