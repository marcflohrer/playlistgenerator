// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MusicOrganizer.Services;

namespace MusicOrganizer.Models;

public record PlaylistSongs(string NormalizedTitle, List<Mp3Info> Mp3Infos);

public record Mp3DirectoryInfo(DirectoryInfo DirectoryInfo);

public record SongLocation(string NormalizedTitle, Mp3Info Mp3Info);

public record SongsOfArtists(IDictionary<string, SongsByTitle> Value);

public record SongsByTitle(IDictionary<string, Mp3Info> Value);

public record SongLocations(string NormalizedTitle, List<Mp3Info> Mp3Infos);

public record Mp3Info(
                    string FilePath,
                    long SizeInBytes,
                    string MainArtist,
                    string[] Interpret,
                    string[] AlbumArtists,
                    string Title,
                    int Number,
                    string AmazonId,
                    int Year,
                    int DurationSeconds);

public static class Mp3InfoExtensions
{
    public static bool IsMissing(this Mp3Info mp3Info) => string.IsNullOrEmpty(mp3Info.FilePath)
            || !File.Exists(mp3Info.FilePath);

    public static string NormalizedMainInterpret(this Mp3Info mp3Info, IList<MusicBrainzTagMap> tagMaps)
        => Mp3TagService.NormalizeSongTag(mp3Info.MainArtist, tagMaps, NormalizeMode.StrictArtist);

    public static string NormalizedTitle(this Mp3Info mp3Info, IList<MusicBrainzTagMap> tagMaps)
        => Mp3TagService.NormalizeSongTag(mp3Info.Title, tagMaps, NormalizeMode.Strict);
}
