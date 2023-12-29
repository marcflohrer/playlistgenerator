using MusicOrganizer.Models;
using MusicOrganizer.Services;
using static MusicOrganizer.Services.Mp3TagService;

namespace MusicOrganizer.Extensions;

public static class FileTagsExtensions
{
    public static SongsOfArtists ToSongsOfArtist(this IList<FileTags> fileTags, IList<MusicBrainzTagMap> tagMaps, FileInfo? logFile)
    {
        var songsOfArtists = new Dictionary<string, SongsByTitle>();
        foreach (var fileTag in fileTags)
        {
            var mainInterpret = fileTag.Mp3Info.NormalizedMainInterpret(tagMaps)!;
            if (!songsOfArtists.TryGetValue(mainInterpret, out var songsOfMainArtist))
            {
                songsOfMainArtist = new SongsByTitle(new Dictionary<string, Mp3Info>());
                songsOfArtists.Add(mainInterpret, songsOfMainArtist);
            }
            var normalizedTitle = fileTag.Mp3Info.NormalizedTitle(tagMaps);
            if (!songsOfMainArtist.Value.TryGetValue(normalizedTitle, out var songsByTitle))
            {
                songsOfMainArtist.Value.Add(normalizedTitle, fileTag.Mp3Info);
            }
            else
            {
                Logger.WriteLine(logFile, $"Duplicate songs found: {songsByTitle.FilePath} vs {fileTag.Mp3Info.FilePath}. Ignoring the latter.");
            }
        }
        return new SongsOfArtists(songsOfArtists);
    }
}
