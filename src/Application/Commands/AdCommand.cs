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
        (int id, string ip) = UtilFuncs.ParseAccountStr(Params[0]);

        long deposit = long.Parse(Params[1]);
        if(ip == UtilFuncs.GetLocalIPAddress())
        {
            var accRepo = new AccountRepository(Database);
            Account acc = accRepo.SelectById(id);
            accRepo.Update(acc, new Account
            {
                Id = id,
                Balance = acc.Balance + deposit
            });
        }
        else
        {
            BankConnection con = new(ip);

            con.AD(id, deposit);
        }

        Session.WriteLine("AD");
    }

}