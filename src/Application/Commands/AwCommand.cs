namespace P2PBank.Application.Commands;

using P2PBank.Application.Interface;
using P2PBank.Utils;
using P2PBank.Data.Interface;
using P2PBank.Application.Interface.Models;

public class AwCommand : Command
{
    private IAccountRepository accountRepository;

    public AwCommand(IAccountRepository accountRepository, Log log) : base(log)
    {
        Name = "AW";
        this.accountRepository = accountRepository;
    }

    public override void InternalExecute()
    {
        EnsusreParams(2);

        (int, string)? accountTuple = UtilFuncs.ParseAccountStr(Params[0]);
        if(!accountTuple.HasValue)
        {
            throw new UnifiedMessageException("Invalid account format.");
        }
        (int id, string ip) = accountTuple.Value;

        long withdraw = long.Parse(Params[1]);
        if(ip == UtilFuncs.GetLocalIPAddress())
        {
            Account acc = accountRepository.SelectById(id);
            accountRepository.Update(acc, new Account
            {
                Id = id,
                Balance = acc.Balance - withdraw
            });
        }
        else
        {
            BankConnection con = new(ip);

            con.AW(id, withdraw);
        }

        Session.WriteLine("AW");
    }

}