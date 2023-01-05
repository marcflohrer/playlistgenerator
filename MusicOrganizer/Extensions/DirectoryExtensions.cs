using System;
using System.IO;

namespace MusicOrganizer.Extensions;

public static class DirectoryExtensions
{
    public static int DeleteEmptySubDirectories(this DirectoryInfo directoryInfo)
    {
        var deletedEmptyDirs = 0;
        var directories = new Stack<string>(Directory.GetDirectories(directoryInfo.FullName));
        while (directories.Count > 0)
        {
            var directory = directories.Pop();
            if (Directory.GetFiles(directory).Length == 0 &&
                Directory.GetDirectories(directory).Length == 0)
            {
                Directory.Delete(directory, false);
                deletedEmptyDirs++;
            }
            else
            {
                foreach (var subDirectory in Directory.GetDirectories(directory))
                {
                    directories.Push(subDirectory);
                }
            }
        }
        return deletedEmptyDirs;
    }
}

