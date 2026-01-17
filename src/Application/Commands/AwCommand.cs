using System.Net;
using System.Net.Sockets;
using Presentation;
using Utils;
using Data;
using Application.Models;
using System.Data.Common;

namespace Application.Commands;

public class AwCommand : Command
{

    public override void InternalExecute()
    {
        EnsusreParams(2);

        (int, string)? accountTuple = UtilFuncs.ParseAccountStr(Params[0]);
        if(!accountTuple.HasValue)
        {
            throw new UnifiedMessageException("Invalid account format.");
        }
        (int id, string ip) = accountTuple.Value;

        long withdraw = long.Parse(Params[1]);
        if(ip == UtilFuncs.GetLocalIPAddress())
        {
            var accRepo = new AccountRepository(Database);
            Account acc = accRepo.SelectById(id);
            accRepo.Update(acc, new Account
            {
                Id = id,
                Balance = acc.Balance - withdraw
            });
        }
        else
        {
            BankConnection con = new(ip);

            con.AW(id, withdraw);
        }

        Session.WriteLine("AW");
    }

}