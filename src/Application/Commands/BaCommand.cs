namespace P2PBank.Application.Commands;

using P2PBank.Application.Interface;
using P2PBank.Data.Interface;
using P2PBank.Utils;

public class BaCommand : Command
{
    private IAccountRepository accountRepository;

    public BaCommand(IAccountRepository accountRepository, Log log) : base(log)
    {
        Name = "BA";
        this.accountRepository = accountRepository;
    }

    public override void InternalExecute()
    {
        EnsusreParams(0);

        long balance;
        try
        {
            balance = accountRepository.TotalBalance;
        }
        catch
        {
            throw new UnifiedMessageException("Failed to calculate the total bank balance.");
        }

        Session.WriteLine($"BA {balance}");
    }

}