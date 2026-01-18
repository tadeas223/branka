namespace P2PBank.Presentation.Interface;

public interface ISession: IDisposable
{
    public Guid SessionId {get;}
    public bool Connected {get;}

    public void Write(string msg);
    public void WriteLine(string msg);
    public Task WriteAsync(string msg);
    public Task WriteLineAsync(string msg);

    public void WriteDontLog(string msg);
    public void WriteLineDontLog(string msg);
    public Task WriteDontLogAsync(string msg);
    public Task WriteLineDontLogAsync(string msg);

    public ManualResetEvent ProcessingRequest {get;}
    public string HostIdentifier{get;}
}