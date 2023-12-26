// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MusicOrganizer.Services;

public class PlaylistService
{
    public static string? CreatePlaylistEntry(DirectoryInfo directoryInfo, FileInfo mp3File, FileInfo? logFile)
    {
        var fullPath = new Uri(Path.GetFullPath(mp3File.FullName!), UriKind.Absolute);
        var relRoot = new Uri(Path.GetFullPath(directoryInfo.FullName + Path.DirectorySeparatorChar), UriKind.Absolute);

        var relativePath = relRoot.MakeRelativeUri(fullPath).ToString();
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
