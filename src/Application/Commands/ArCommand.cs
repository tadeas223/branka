using System.Net;
using System.Net.Sockets;
using Presentation;
using Utils;
using Data;
using Application.Models;

namespace Application.Commands;

public class ArCommand : Command
{

    public override void InternalExecute()
    {
        (int id, string ip) = UtilFuncs.ParseAccountStr(Params[0]);

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

        Session.WriteLine("Ar");
    }

}