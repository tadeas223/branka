namespace P2PBank.Application.Interface;

using P2PBank.Presentation.Interface;
using P2PBank.Utils;

using WorkDispatcher;

public abstract class Command: IWorkerTask
{
    public List<string> Params {get; set;} = new();
    public string? Name {get; protected set;}
    protected Log log;
    private ISession? session = null;
    public ISession Session
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
    
    public Command(Log log)
    {
        Name = null;
        this.log = log;
    }

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
        }
        catch(Exception e)
        {
            Session.WriteLine("ER Something went wrong on the server.");
            log.Error(e.ToString());
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