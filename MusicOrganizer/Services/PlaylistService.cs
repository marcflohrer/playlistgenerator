using System;
using System.Web;
using MusicOrganizer.Extensions;

namespace MusicOrganizer.Services;

public class PlaylistService
{
    public static string? CreatePlaylistEntry(DirectoryInfo directoryInfo, FileInfo mp3File, FileInfo? logFile)
    {
        Uri fullPath = new Uri(Path.GetFullPath(mp3File.FullName!), UriKind.Absolute);
        Uri relRoot = new Uri(Path.GetFullPath(directoryInfo.FullName + Path.DirectorySeparatorChar), UriKind.Absolute);

        string? relativePath = relRoot.MakeRelativeUri(fullPath).ToString();
        if (!string.IsNullOrWhiteSpace(relativePath))
        {
            var decoded = Uri.UnescapeDataString(relativePath);
            if (!string.IsNullOrWhiteSpace(decoded))
            {
                return decoded;
            }
        }
        else
        {
            Logger.WriteLine(logFile, $"File is not in directory {fullPath}");
        }
        return null;
    }
}

