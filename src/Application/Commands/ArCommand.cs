namespace P2PBank.Application.Commands;

using P2PBank.Application.Interface;
using P2PBank.Utils;
using P2PBank.Application;
using P2PBank.Data.Interface;
using P2PBank.Application.Interface.Models;
using P2PBank.Presentation.Interface;
using P2PBank.Presentation.Tcp;

public class ArCommand : Command
{
    private IAccountRepository accountRepository;
    private ServerConfig serverConfig;

    public ArCommand(IAccountRepository accountRepository, ServerConfig serverConfig, Log log) : base(log)
    {
        Name = "AR";
        this.accountRepository = accountRepository;
        this.serverConfig = serverConfig;
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
            Account acc;
            try
            {
                acc = accountRepository.SelectById(id);
            }
            catch
            {
                throw new UnifiedMessageException("Account was not found.");
            }

            if(acc.Balance != 0)
            {
                throw new UnifiedMessageException("Nelze smazat bankovní účet na kterém jsou finance.");
            }

            try
            {
                accountRepository.Delete(acc);
            }
            catch
            {
                throw new UnifiedMessageException("Account could not be deleted from the bank database.");
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
                con.AR(id);
            }
            catch
            {
                throw new UnifiedMessageException("Account could not be deleted from a remote bank or failed to connect to a remote bank.");
            }
        }

        Session.WriteLine("AR");
    }

}