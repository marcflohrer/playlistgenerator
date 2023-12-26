// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MusicOrganizer.Models
{
    public record AppOptions(
        Mp3DirectoryInfo MusicDirectory,
        FileInfo LogFile,
        FileInfo ResumeMainFiles,
        FileInfo ResumeMainTags,
        FileInfo? DeletionScript,
        IList<FileInfo> CsvPlaylistFiles);

    public static class AppOptionsExtensions
    {
        public static FileInfo ToPlaylistFile(this AppOptions appOptions, FileInfo csvPlaylistFile)
        {
            return new FileInfo(Path.Combine(appOptions.MusicDirectory.DirectoryInfo.FullName, csvPlaylistFile.Name.Replace(".csv", ".m3u")));
        }
    }
}

