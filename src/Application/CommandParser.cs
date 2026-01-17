using Application.Commands;

namespace Application;

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
            erCmd.Params.Add("Failed to parse command name.");
            return erCmd;
        }

        List<string> argList= new();
        string name = args[0];
        argList.AddRange(args[1..]);

        if(!commands.ContainsKey(name))
        {
            Command erCmd = new ErCommand();
            erCmd.Params.Add("Command does not exist.");
            return erCmd;
        }
        
        Command? cmd = (Command?)Activator.CreateInstance(commands[name]);
        if(cmd == null)
        {
            Command erCmd = new ErCommand();
            erCmd.Params.Add("Could not execute command because of an internal error.");
            return erCmd;
        }
        cmd.Params.AddRange(argList);

        return cmd;
    }
}