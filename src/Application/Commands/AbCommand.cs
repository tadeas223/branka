namespace P2PBank.Application.Commands;

using P2PBank.Application.Interface;
using P2PBank.Utils;
using P2PBank.Data.Interface;
using P2PBank.Application.Interface.Models;

public class AbCommand: Command
{
    private IAccountRepository accountRepository;
    public AbCommand(IAccountRepository accountRepository, Log log) : base(log)
    {
        Name = "AB";
        this.accountRepository = accountRepository;
    }

    public override void InternalExecute()
    {
        EnsusreParams(1);

        (int, string)? accountTuple = UtilFuncs.ParseAccountStr(Params[0]);
        if(!accountTuple.HasValue)
        {
            throw new UnifiedMessageException("Invalid account format.");
        }
        (int id, string ip) = accountTuple.Value;

        long balance = 0;

        if(ip == UtilFuncs.GetLocalIPAddress())
        {
            try
            {
                Account acc = accountRepository.SelectById(id);
                balance = acc.Balance;
            }
            catch
            {
                throw new UnifiedMessageException("Account was not found.");
            }
        }
        else
        {
            BankConnection con = new(ip);

            balance = con.AB(id);
        }

        Session.WriteLine($"AB {balance}");
    }
}