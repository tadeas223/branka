using Data;
using Application.Models;
using Utils;

namespace Application.Commands;

public class AbCommand: Command
{
    public override void InternalExecute()
    {
        (int id, string ip) = UtilFuncs.ParseAccountStr(Params[0]);

        long balance = 0;

        if(ip == UtilFuncs.GetLocalIPAddress())
        {
            var accRepo = new AccountRepository(Database);
            Account acc = accRepo.SelectById(id);
            balance = acc.Balance;
        }
        else
        {
            BankConnection con = new(ip);

            balance = con.AB(id);
        }

        Session.WriteLine("AD");
    }
}