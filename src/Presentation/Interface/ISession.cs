namespace P2PBank.Presentation.Interface;

public interface ISession: IDisposable
{
    public Guid SessionId {get;}
    public bool Connected {get;}

    public void Write(string msg);
    public void WriteLine(string msg);
    public Task WriteAsync(string msg);
    public Task WriteLineAsync(string msg);
}