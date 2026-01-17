namespace P2PBank.Utils;

using System.IO;

public class Log
{
    public string? FilePath{get; set;}

    public void Warn(string msg)
    {
        string text = $"[{DateTime.Now}  warn] {msg}\n";
        
        Console.Write(text);
        WriteToFile(FilePath, text);
    }

    public void Info(string msg)
    {
        string text = $"[{DateTime.Now}  info] {msg}\n";
        
        Console.Write(text);
        WriteToFile(FilePath, text);
    }

    public void Error(string msg)
    {
        string text = $"[{DateTime.Now} error] {msg}\n";
        
        Console.Write(text);
        WriteToFile(FilePath, text);
    }

    private static void WriteToFile(string? path, string text)
    {
        if(path != null)
        {
            try
            {
                File.AppendAllText(path, text);
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now} error]: failed to write save log");
            }
        }

    }
}