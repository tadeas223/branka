using Presentation;

namespace Application.Commands;

public class ErCommand : Command
{

    public override void InternalExecute()
    {
        EnsusreParams(1);

        Session.WriteLine($"ER {Params[0]}");
    }
}