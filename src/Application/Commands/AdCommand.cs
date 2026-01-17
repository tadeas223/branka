namespace P2PBank.Application.Commands;

using P2PBank.Application.Interface;
using P2PBank.Utils;
using P2PBank.Data.Interface;
using P2PBank.Application.Interface.Models;

public class AdCommand : Command
{
    private IAccountRepository accountRepository;

    public AdCommand(IAccountRepository accountRepository, Log log) : base(log)
    {
        Name = "AD";
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

        long deposit = long.Parse(Params[1]);
        if(ip == UtilFuncs.GetLocalIPAddress())
        {
            Account acc;
            try
            {
                acc = accountRepository.SelectById(id);
            }
            catch
            {
                throw new UnifiedMessageException("Account was not found.");
            }

            try
            {
                accountRepository.Update(acc, new Account
                {
                    Id = id,
                    Balance = acc.Balance + deposit
                });
            }
            catch
            {
                throw new UnifiedMessageException("Failed to deposit.");
            }
        }
        else
        {
            BankConnection con = new(ip);

            con.AD(id, deposit);
        }

        Session.WriteLine("AD");
    }

}