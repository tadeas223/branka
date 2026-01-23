namespace P2PBank.Presentation.Interface;

public interface IServer: IDisposable
{
    public Task StartAsync();
    public void RegisterSession(ISession session);
    public void TerminateSession(ISession session, string reason);
    public IEnumerable<ISession> GetActiveSessions();
}