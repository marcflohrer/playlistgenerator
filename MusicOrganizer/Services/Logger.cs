using System;
using System.IO;

namespace MusicOrganizer.Services;

public static class Logger
{
    public static void WriteLine(FileInfo? output, string text)
    {
        if (text == null)
        {
            return;
        }
        if (output == null)
        {
            Console.WriteLine(text);
        }
        else
        {
            if (!File.Exists(output.FullName))
            {
                File.WriteAllText(output.FullName, text + Environment.NewLine);
            }
            else
            {
                File.AppendAllLines(output.FullName, new List<string> { text });
            }
        }
    }
}

