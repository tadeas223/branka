using Presentation;
using WorkDispatcher;
using Data;
using Utils;

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

    public abstract void InternalExecute();

    public void Execute()
    {
        try
        {
            InternalExecute();
        }
        catch (UnifiedMessageException e)
        {
            Session.WriteLine($"ER {e.Message}");
            Log.Info($"{Session.Socket.RemoteEndPoint} {e}");
        }
        catch(Exception e)
        {
            Session.WriteLine("ER Uncaugnt internal error.");
            Log.Error($"{Session.Socket.RemoteEndPoint} uncaught internal error: {e}");
        }
    }

    public void EnsusreParams(int paramCount)
    {
        if(Params.Count != paramCount)
        {
            throw new UnifiedMessageException("Invalid number of arguments.");
        }
    }
}