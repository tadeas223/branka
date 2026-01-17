
using System.Net;
using System.Net.Sockets;
using Presentation;
using Utils;
using Data;
using Application.Models;
using System.Xml;

namespace Application.Commands;

public class BaCommand : Command
{

    public override void InternalExecute()
    {
        AccountRepository accRepo = new(Database);
        long balance = accRepo.TotalBalance;

        Session.WriteLine($"BA {balance}");
    }

}