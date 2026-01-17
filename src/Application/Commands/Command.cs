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
        catch (BankException e)
        {
            Session.WriteLine($"ER {e.Message}");
            Log.Error($"{Session.Socket.RemoteEndPoint} remote bank retured an error that said \"{e}\" ");
        }
        catch (DatabaseException e)
        {
            Session.WriteLine($"ER {e.Message}");
            Log.Error($"{Session.Socket.RemoteEndPoint} database error: {e}");
        }
        catch(Exception e)
        {
            Session.WriteLine("ER uncaugnt internal error");
            Log.Error($"{Session.Socket.RemoteEndPoint} uncaught internal error: {e}");
        }
    }
}