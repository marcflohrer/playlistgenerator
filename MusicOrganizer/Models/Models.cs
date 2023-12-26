// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MusicOrganizer.Models;

public record PlaylistSongs(string NormalizedSongName, List<Mp3Info> Mp3Infos);

public record Mp3DirectoryInfo(DirectoryInfo DirectoryInfo);

public record SongLocation(string NormalizedSongName, Mp3Info Mp3Info);

public record SongLocations(string NormalizedSongName, List<Mp3Info> Mp3Infos);

public record Mp3Info(
                    string FilePath,
                    string Md5Hash,
                    long SizeInBytes,
                    string[] Interpret,
                    string[] AlbumArtists,
                    string Title,
                    string NormalizedSongName,
                    int Number,
                    string AmazonId,
                    int Year,
                    int DurationSeconds);

public static class Mp3InfoExtensions
{
    public static bool IsMissing(this Mp3Info mp3Info) => string.IsNullOrEmpty(mp3Info.FilePath)
            || !File.Exists(mp3Info.FilePath);
}
