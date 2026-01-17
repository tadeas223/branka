using System.Net;
using System.Net.Sockets;
using Presentation;
using Utils;
using Data;
using Application.Models;
using System.Xml;

namespace Application.Commands;

public class BnCommand : Command
{

    public override void InternalExecute()
    {
        EnsusreParams(0);

        AccountRepository accRepo = new(Database);
        int clients = accRepo.ClientCount;

        Session.WriteLine($"BN {clients}");
    }

}