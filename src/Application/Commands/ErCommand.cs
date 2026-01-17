using Presentation;

namespace Application.Commands;

public class ErCommand : Command
{

    public override void InternalExecute()
    {
        Session.WriteLine($"ER {Params[0]}");
    }
}