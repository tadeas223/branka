using Presentation;

namespace Application.Commands;

public class TestCommand : Command
{

    public override void Execute()
    {
        Session.WriteLine("ahoj");
    }
}