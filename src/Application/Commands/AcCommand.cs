using Data;
using Application.Models;
using Utils;

namespace Application.Commands;

public class AcCommand: Command
{
    public override void InternalExecute()
    {
        AccountRepository accRepo = new(Database);
        Account account = new();
        account.Ip = UtilFuncs.GetLocalIPAddress();
        
        accRepo.Insert(account);


        Session.WriteLine($"AC {account}");
    }
}