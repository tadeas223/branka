using Presentation;
using WorkDispatcher;

namespace Application.Commands;

public abstract class Command: IWorkerTask
{
    public List<string> Params {get; set;} = new();

    private TcpSession? session = null;
    private Database? database = null;

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
    
    public Database Database
    {
        get
        {
            if(database== null)
            {
                throw new NullReferenceException("database was not set");
            }
            return database;
        }

        set
        {
            database = value;
        }
    }

    public Command(){}

    public abstract void Execute();
}