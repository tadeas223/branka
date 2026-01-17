namespace P2PBank.Application.Commands;

using P2PBank.Application.Interface;
using P2PBank.Utils;
using P2PBank.Application;
using P2PBank.Data.Interface;
using P2PBank.Application.Interface.Models;

public class ArCommand : Command
{
    private IAccountRepository accountRepository;

    public ArCommand(IAccountRepository accountRepository, Log log) : base(log)
    {
        Name = "AR";
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

        if(ip == UtilFuncs.GetLocalIPAddress())
        {
            Account acc = accountRepository.SelectById(id);
            accountRepository.Delete(acc);
        }
        else
        {
            BankConnection con = new(ip);

            con.AD(id);
        }

        Session.WriteLine("AR");
    }

}