using System.Net;
using System.Net.Sockets;
using Application.Models;
using Data;
using Presentation;
using Utils;

namespace Application.Commands;

public class AdCommand : Command
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

        long deposit = long.Parse(Params[1]);
        if(ip == UtilFuncs.GetLocalIPAddress())
        {
            var accRepo = new AccountRepository(Database);
            Account acc;
            try
            {
                acc = accRepo.SelectById(id);
            }
            catch
            {
                throw new UnifiedMessageException("Account was not found.");
            }

            try
            {
                accRepo.Update(acc, new Account
                {
                    Id = id,
                    Balance = acc.Balance + deposit
                });
            }
            catch
            {
                throw new UnifiedMessageException("Failed to deposit.");
            }
        }
        else
        {
            BankConnection con = new(ip);

            con.AD(id, deposit);
        }

        Session.WriteLine("AD");
    }

}