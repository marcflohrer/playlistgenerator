// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MusicOrganizer.Models
{
    public record AppOptions(List<Mp3DirectoryInfo> MusicDirectories,
        FileInfo LogFile,
        FileInfo ResumeMainFiles,
        FileInfo ResumeMainTags,
        FileInfo ResumePlaylistFiles,
        FileInfo ResumePlaylistTags,
        FileInfo? DeletionScript,
        FileInfo? PlaylistFile,
        FileInfo? CsvInputPlaylistFile);
}

