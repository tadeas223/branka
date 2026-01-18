namespace P2PBank.Application.Commands;

using P2PBank.Application.Interface;
using P2PBank.Data.Interface;
using P2PBank.Utils;

public class BnCommand : Command
{
    private IAccountRepository accountRepository;

    public BnCommand(IAccountRepository accountRepository, Log log): base(log)
    {
        Name = "BN";
        this.accountRepository = accountRepository;
    }

    public override void InternalExecute()
    {
        EnsusreParams(0);
        
        int clients;
        try
        {
            clients = accountRepository.ClientCount;
        }
        catch
        {
            throw new UnifiedMessageException("Failed to calculate the total client count.");
        }

        Session.WriteLine($"BN {clients}");
    }

}