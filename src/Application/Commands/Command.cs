using Presentation;
using WorkDispatcher;

namespace Application.Commands;

public abstract class Command: IWorkerTask
{
    public List<string> Params {get; set;} = new();

    private TcpSession? session = null;

    public TcpSession Session
    {
        get
        {
            if(session == null)
            {
                throw new NullReferenceException("session was not set");
            }
            return session;
        }

        set
        {
            session = value;
        }
    }

    public Command(){}

    public abstract void Execute();
}