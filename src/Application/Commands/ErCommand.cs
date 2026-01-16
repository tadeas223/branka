using Presentation;

namespace Application.Commands;

public class ErCommand : Command
{

    public override void Execute()
    {
        Session.WriteLine($"ER {Params[0]}");
    }
}