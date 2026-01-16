namespace Utils;

using System.ComponentModel;
using System.IO;

public class Log
{
    private static Log? instance = null;
    public static Log Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new();
            }

            return instance;
        }
    }

    public string? FilePath{get; set;}

    private Log() {}

    public void _Warn(string msg)
    {
        string text = $"[{DateTime.Now} warn] {msg}\n";
        
        Console.Write(text);
        WriteToFile(FilePath, text);
    }

    public void _Info(string msg)
    {
        string text = $"[{DateTime.Now} info] {msg}\n";
        
        Console.Write(text);
        WriteToFile(FilePath, text);
    }

    public void _Error(string msg)
    {
        string text = $"[{DateTime.Now} error] {msg}\n";
        
        Console.Write(text);
        WriteToFile(FilePath, text);
    }

    public static void Warn(string msg)
    {
        Instance._Warn(msg);
    }
    public static void Info(string msg)
    {
        Instance._Info(msg);
    }
    public static void Error(string msg)
    {
        Instance._Error(msg);
    }

    private void WriteToFile(string? path, string text)
    {
        if(FilePath != null)
        {
            try
            {
                File.AppendAllText(FilePath, text);
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now}error]: failed to write save log");
            }
        }

    }
}