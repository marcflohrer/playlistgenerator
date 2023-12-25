namespace MusicOrganizer.Services;

public static class Logger
{
    public static void WriteLine(FileInfo? logFile, string text)
    {
        if (text == null)
        {
            return;
        }
        if (logFile == null)
        {
            Console.WriteLine(text);
        }
        else
        {
            if (!File.Exists(logFile.FullName))
            {
                File.WriteAllText(logFile.FullName, text + Environment.NewLine);
            }
            else
            {
                File.AppendAllLines(logFile.FullName, new List<string> { text });
            }
        }
    }
}

