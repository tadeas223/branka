using System.Net;
using System.Net.Sockets;
using Presentation;
using Utils;

namespace Application.Commands;

public class BcCommand : Command
{

    public override void InternalExecute()
    {
        string? ip = UtilFuncs.GetLocalIPAddress();

        Session.WriteLine($"BC {ip}");
    }

}