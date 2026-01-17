namespace P2PBank.Application.Interface;

using Microsoft.Extensions.DependencyInjection;
using P2PBank.Application.Commands;


public class CommandFactory : ICommandFactory
{
    IServiceProvider provider;

    public CommandFactory(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public Command Create(string cmdString)
    {
        string[] split = cmdString.Split(' ');
        if(split.Length < 1)
        {
            ErCommand? erCommand = provider.GetService<ErCommand>();
            if(erCommand == null)
            {
                throw new Exception("ErCommand service is missing");
            }

            erCommand.Params.Add("Failed to parse command name.");

            return erCommand;
        }

        string name = split[0];

        Command? cmd = null;
        switch(name.ToUpper())
        {
            case "AB":
                cmd = provider.GetService<AbCommand>();
                break;
            case "AC":
                cmd = provider.GetService<AcCommand>();
                Console.Write(cmd!.Name);
                break;
            case "AR":
                cmd = provider.GetService<ArCommand>();
                break;
            case "AD":
                cmd = provider.GetService<ArCommand>();
                break;
            case "AW":
                cmd = provider.GetService<AwCommand>();
                break;
            case "BA":
                cmd = provider.GetService<BaCommand>();
                break;
            case "BC":
                cmd = provider.GetService<BcCommand>();
                break;
            case "BN":
                cmd = provider.GetService<BnCommand>();
                break;
        }

        if(cmd == null) 
        {
            ErCommand? erCommand = provider.GetService<ErCommand>();
            if(erCommand == null)
            {
                throw new Exception("ErCommand service is missing");
            }

            erCommand.Params.Add("Command was not found.");

            return erCommand;
        }

        return cmd;
    }
}