using MusicOrganizer.Extensions;
using MusicOrganizer.Models;
using static MusicOrganizer.Services.Mp3TagService;

namespace MusicOrganizer.Services;

public static class M3uPlaylistCreator
{
    public static void ToM3uPlaylist(
        this IList<Mp3Info> playlistEntries,
        IList<MusicBrainzTagMap> tagMaps,
        ScanResult mainDirScanResult,
        FileInfo playList,
        Mp3DirectoryInfo mainDir,
        FileInfo? logFile)
    {
        var mp3InfoPlaylist = new List<FileTags>();
        foreach (var playlistEntry in playlistEntries)
        {
            var songsOfArtist = GetSongsOfArtistInLocalLibrary(mainDirScanResult.SongsOfArtists, playlistEntry.NormalizedMainInterpret(tagMaps));
            var matchingSongInLocalLibrary = GetSongsForTitleForGivenArtist(songsOfArtist, playlistEntry.NormalizedTitle(tagMaps));
            if (matchingSongInLocalLibrary != null)
            {
                mp3InfoPlaylist.Add(new FileTags(string.Empty, matchingSongInLocalLibrary!));
            }
            else
            {
                Logger.WriteLine(logFile, $"No match found '{playlistEntry.Title}' by '{string.Join(',', playlistEntry.Interpret)}' from '{playlistEntry.Year}'");
            }
        }

        NormalizeFileTags(mp3InfoPlaylist, tagMaps)
            .RemoveDuplicates()
            .CreatePlaylistFile(playList, mainDir.DirectoryInfo, logFile);
    }

    private static SongsByTitle GetSongsOfArtistInLocalLibrary(SongsOfArtists songsOfArtists, string mainArtist)
    {
        var found = songsOfArtists.Value.TryGetValue(mainArtist, out var songsByTitle);
        if (!found)
        {
            songsByTitle = new SongsByTitle(Value: new Dictionary<string, Mp3Info>());
        }
        return songsByTitle!;
    }

    private static Mp3Info? GetSongsForTitleForGivenArtist(SongsByTitle songsByTitle, string normalizedTitle) 
        => !songsByTitle.Value.TryGetValue(normalizedTitle, out var matchingSongInLocalLibrary)
                ? null
                : matchingSongInLocalLibrary;
}

