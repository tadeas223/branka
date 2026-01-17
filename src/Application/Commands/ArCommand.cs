using System.Net;
using System.Net.Sockets;
using Presentation;
using Utils;
using Data;
using Application.Models;
using System.Data.Common;

namespace Application.Commands;

public class ArCommand : Command
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

        if(ip == UtilFuncs.GetLocalIPAddress())
        {
            var accRepo = new AccountRepository(Database);
            Account acc = accRepo.SelectById(id);
            accRepo.Delete(acc);
        }
        else
        {
            BankConnection con = new(ip);

            con.AD(id);
        }

        Session.WriteLine("AR");
    }

}