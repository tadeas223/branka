namespace P2PBank.Application.Commands;

using P2PBank.Utils;
using P2PBank.Data.Interface;
using P2PBank.Application.Interface.Models;
using P2PBank.Application.Interface;

public class AcCommand: Command
{
    private IAccountRepository accountRepository;

    public AcCommand(IAccountRepository accountRepository, Log log) : base(log)
    {
        Name = "AC";
        this.accountRepository = accountRepository;
    }

    public override void InternalExecute()
    {
        EnsusreParams(0);

        Account account = new();
        account.Ip = UtilFuncs.GetLocalIPAddress();

        accountRepository.Insert(account);

        Session.WriteLine($"AC {account}");
    }
}