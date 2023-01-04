using System;
using System.IO;

namespace MusicOrganizer.Extensions;

public static class DirectoryExtensions
{
    public static int DeleteEmptySubDirectories(this DirectoryInfo directoryInfo)
    {
        var deletedEmptyDirs = 0;
        foreach (var directory in Directory.GetDirectories(directoryInfo.FullName))
        {
            deletedEmptyDirs += DeleteEmptySubDirectories(new DirectoryInfo(directory));
            if (Directory.GetFiles(directory).Length == 0 &&
                Directory.GetDirectories(directory).Length == 0)
            {
                Directory.Delete(directory, false);
                deletedEmptyDirs++;
            }
        }
        return deletedEmptyDirs;
    }
}

