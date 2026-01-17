using System.Reflection.Metadata.Ecma335;
using Application.Commands;

namespace Presentation;

public static class CommandParser
{
    public static Command Parse(string cmdString)
    {
        string[] args = cmdString.Split(" ");
        List<string> argList= new(args);
        if(argList.Count < 1)
        {
            Command erCmd = new ErCommand();
            erCmd.Params.Add("invalid command");
            return erCmd;
        }

        string name = argList[0];
        argList.RemoveAt(0);
        
        Command cmd;
        switch(name.ToUpper())
        {
            case "BR":
            cmd = new BcCommand();
            break;

            default:
                cmd = new ErCommand();
                cmd.Params.Add("invalid command");
                break;

        }

        return cmd;
    }
}