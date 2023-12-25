using MusicOrganizer.Extensions;
using MusicOrganizer.Models;
using static MusicOrganizer.Services.Mp3TagService;

namespace MusicOrganizer.Services;

public static class M3uPlaylistCreator
{
    public static void ToM3uPlaylist(
        this IList<PlaylistEntry> playlistEntries,
        ScanResult mainDirScanResult,
        FileInfo playList,
        Mp3DirectoryInfo mainDir,
        FileInfo? logFile)
    {
        var mp3InfoPlaylist = new List<FileTags>();
        foreach (var playlistEntry in playlistEntries)
        {
            var matchingFileTagsInterprets = mainDirScanResult.FileTags
                                        .Where(sr => sr.Mp3Info.Interpret
                                                .ToOrderedList()
                                            .Select(i => i.NormalizeSongTag(NormalizeMode.Strict))
                                                .SequenceEqual(playlistEntry.Interpret
                                                    .ToOrderedList()
                                                    .Select(i => i.NormalizeSongTag(NormalizeMode.Strict))))
                                        .ToList();
            var matchingFileTags = new List<FileTags>();
            if (matchingFileTagsInterprets.Count == 0)
            {
                var interprets = playlistEntry.Interpret;
                foreach (var interpret in interprets)
                {
                    var normalizedInterpret = interpret.NormalizeSongTag(NormalizeMode.Strict);
                    foreach (var fileTag in mainDirScanResult.FileTags)
                    {
                        foreach (var fileTagInterpret in fileTag.Mp3Info.Interpret)
                        {
                            var normalizedFileTagInterpret = fileTagInterpret.NormalizeSongTag(NormalizeMode.Strict);
                            if (normalizedFileTagInterpret.StartsWith(normalizedInterpret) ||
                                normalizedInterpret.StartsWith(normalizedFileTagInterpret))
                            {
                                matchingFileTagsInterprets.Add(fileTag);
                                continue;
                            }
                        }
                    }
                }
            }

            if (matchingFileTagsInterprets.Count != 0)
            {
                matchingFileTags = matchingFileTagsInterprets
                                        .Where(sr => sr.Mp3Info.NormalizedSongName.Contains(playlistEntry.NormalizedSongName)
                                            || playlistEntry.NormalizedSongName.Contains(sr.Mp3Info.NormalizedSongName))
                                        .ToList();
            }

            if (matchingFileTags == null || matchingFileTags.Count == 0)
            {
                Logger.WriteLine(logFile, $"Nothing found for Song {playlistEntry.Title} by {string.Join(',', playlistEntry.Interpret)} from {playlistEntry.Year}");
                continue;
            }

            mp3InfoPlaylist.Add(matchingFileTags.FirstOrDefault()!);
        }

        NormalizeFileTags(mp3InfoPlaylist)
            .RemoveDuplicates()
            .CreatePlaylistFile(playList, mainDir.DirectoryInfo, logFile);
    }
}

