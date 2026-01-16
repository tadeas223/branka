using System.Reflection.Metadata.Ecma335;
using Application.Commands;

namespace Presentation;

public static class CommandParser
{
    public static Command Parse(string cmdString)
    {
        return new TestCommand();
    }
}