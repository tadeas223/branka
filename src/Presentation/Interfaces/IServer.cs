namespace P2PBank.Presentation.Interfaces;

public interface IServer: IDisposable
{
    public Task StartAsync();
    public void Start();
    public void RegisterSession(ISession session);
    public void TerminateSession(ISession session, string reason);
}