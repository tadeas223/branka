namespace P2PBank.Application.Commands;

using P2PBank.Application.Interface;
using P2PBank.Utils;
using P2PBank.Data.Interface;
using P2PBank.Application.Interface.Models;
using System.Runtime.CompilerServices;

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

        long withdraw;

        try
        {
            withdraw = long.Parse(Params[1]);
        }
        catch
        {
            throw new UnifiedMessageException("Withdraw must be a positive number.");
        }

        if(ip == UtilFuncs.GetLocalIPAddress())
        {   
            Account acc;
            try
            {
                acc = accountRepository.SelectById(id);
            }
            catch
            {
                throw new UnifiedMessageException("Account cound not be found.");
            }

            try
            {
                accountRepository.Update(acc, new Account
                {
                    Id = id,
                    Balance = acc.Balance - withdraw
                });
            }
            catch
            {
                throw new UnifiedMessageException("Could not update an account in the database.");
            }
        }
        else
        {
            BankConnection con = new(ip);

            try
            {
                con.AW(id, withdraw);
            }
            catch
            {
                throw new UnifiedMessageException("Failed to withdraw from a remote bank or failed to connect to a remote bank.");
            }
        }

        Session.WriteLine("AW");
    }

}