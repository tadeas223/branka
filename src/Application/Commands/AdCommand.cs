namespace P2PBank.Application.Commands;

using P2PBank.Application.Interface;
using P2PBank.Utils;
using P2PBank.Data.Interface;
using P2PBank.Application.Interface.Models;
using P2PBank.Presentation.Interface;
using P2PBank.Presentation.Tcp;

public class AdCommand : Command
{
    private IAccountRepository accountRepository;
    private ServerConfig serverConfig;

    public AdCommand(IAccountRepository accountRepository, ServerConfig serverConfig, Log log) : base(log)
    {
        Name = "AD";
        this.accountRepository = accountRepository;
        this.serverConfig = serverConfig;
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


        long deposit = 0;
        try
        {
            deposit = long.Parse(Params[1]);
        }
        catch
        {
            throw new UnifiedMessageException("Deposit must be a positive number.");
        }

        Validators.ValidateAmount(deposit);
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
            TcpServerConfig? tcpConfig = serverConfig as TcpServerConfig;
            int port = tcpConfig?.Port ?? 65525;
            int timeout = tcpConfig?.ClientTimeout ?? 5;
            BankConnection con = new(ip, port, timeout);

            try
            {
                con.AD(id, deposit);
            }
            catch
            {
                throw new UnifiedMessageException("Failed to deposit to a remote bank.");
            }
        }

        Session.WriteLine("AD");
    }

}