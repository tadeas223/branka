namespace P2PBank.Application.Commands;

using P2PBank.Application.Interface;
using P2PBank.Utils;
using P2PBank.Data.Interface;
using P2PBank.Application.Interface.Models;
using P2PBank.Presentation.Interface;
using P2PBank.Presentation.Tcp;

public class AbCommand: Command
{
    private IAccountRepository accountRepository;
    private ServerConfig serverConfig;
    
    public AbCommand(IAccountRepository accountRepository, ServerConfig serverConfig, Log log) : base(log)
    {
        Name = "AB";
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
            TcpServerConfig? tcpConfig = serverConfig as TcpServerConfig;
            int port = tcpConfig?.Port ?? 65525;
            int timeout = tcpConfig?.ClientTimeout ?? 5;
            BankConnection con = new(ip, port, timeout);

            try
            {
                balance = con.AB(id);
            }
            catch
            {
                throw new UnifiedMessageException("Failed to get balance from a remote bank.");
            }
        }

        Session.WriteLine($"AB {balance}");
    }
}