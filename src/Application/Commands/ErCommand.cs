namespace P2PBank.Application.Commands;

using P2PBank.Application.Interface;
using P2PBank.Utils;

public class ErCommand : Command
{
    public ErCommand(Log log) : base(log)
    {
        Name = "ER";
    }
    public override void InternalExecute()
    {
        EnsusreParams(1);

        Session.WriteLine($"ER {Params[0]}");
    }
}