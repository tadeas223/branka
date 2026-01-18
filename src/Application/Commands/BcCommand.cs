namespace P2PBank.Application.Commands;

using P2PBank.Application.Interface;
using P2PBank.Utils;
using P2PBank.Data.Interface;

public class BcCommand : Command
{
    public BcCommand(Log log) : base(log)
    {
        Name = "BC";
    }

    public override void InternalExecute()
    {
        EnsusreParams(0);
        
        string? ip = UtilFuncs.GetLocalIPAddress();

        Task.Delay(1000).GetAwaiter().GetResult();
        Session.WriteLine($"BC {ip}");
    }

}