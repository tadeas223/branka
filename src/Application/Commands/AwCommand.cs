using System.Net;
using System.Net.Sockets;
using Presentation;
using Utils;
using Data;
using Application.Models;

namespace Application.Commands;

public class AwCommand : Command
{

    public override void InternalExecute()
    {
        (int id, string ip) = UtilFuncs.ParseAccountStr(Params[0]);

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