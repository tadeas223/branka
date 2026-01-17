using System.Reflection.Metadata.Ecma335;
using Application.Commands;
using Application;

namespace Presentation;

public static class CommandParser
{

    private static Dictionary<string, Type>? commands;      
    
    public static Command Parse(string cmdString)
    {
        if(commands == null)
        {
            commands = new();
            commands.Add("BC", typeof(BcCommand));
            commands.Add("AC", typeof(AcCommand));
            commands.Add("AD", typeof(AdCommand));
            commands.Add("AW", typeof(AwCommand));
            commands.Add("AB", typeof(AbCommand));
            commands.Add("AR", typeof(ArCommand));
            commands.Add("BA", typeof(BaCommand));
            commands.Add("BN", typeof(BnCommand));
        }

        string[] args = cmdString.Split(' ');
        
        if(args.Length < 1)
        {
            Command erCmd = new ErCommand();
            erCmd.Params.Add("failed to parse command name");
            return erCmd;
        }

        List<string> argList= new();
        string name = args[0];
        argList.AddRange(args[1..]);

        if(!commands.ContainsKey(name))
        {
            Command erCmd = new ErCommand();
            erCmd.Params.Add("unknown command");
            return erCmd;
        }
        
        Command? cmd = (Command?)Activator.CreateInstance(commands[name]);
        if(cmd == null)
        {
            Command erCmd = new ErCommand();
            erCmd.Params.Add("failed to create command through reflection");
            return erCmd;
        }
        cmd.Params.AddRange(argList);

        return cmd;
    }
}