using MusicOrganizer.Models;
using MusicOrganizer.Services;
using static MusicOrganizer.Services.Mp3TagService;

namespace MusicOrganizer.Extensions;

public static class FindDuplicatesExtensions
{

    public static List<FileTags>? ResumeOrScanMp3Tags(this List<FileInfo>? mp3Files, FileInfo resumeTags, IList<MusicBrainzTagMap> tagMaps, FileInfo logFile)
    {
        List<FileTags>? fileTags;
        if (!resumeTags?.Exists ?? true)
        {
            fileTags = mp3Files?.GetMp3Info(tagMaps, logFile);
            ResumeService.StoreResumeTagsPoint(resumeTags, fileTags ?? []);
        }
        else
        {
            fileTags = ResumeService.LoadResumeTagsPoint(resumeTags) ?? [];
        }
        Logger.WriteLine(logFile, $"{DateTime.Now} Tag Scan on mp3 files in main directory finished");
        return fileTags;
    }

    public static void CreateDeletionScript(this Dictionary<string, SongLocations> duplicatesongs, FileInfo? logFile, FileInfo? deletionScript)
    {
        foreach (var song in duplicatesongs)
        {
            {
                var duplicates = song.Value.Mp3Infos;
                if (duplicates.Count <= 1)
                {
                    continue;
                }
                var orderedDuplicates = duplicates.OrderByDescending(d => d.SizeInBytes);

                orderedDuplicates = AddDuplicateToScriptWhenHasNoAmazonIdAndOthersHaveAmazonId(logFile, deletionScript, orderedDuplicates);

                orderedDuplicates = AddDuplicateToScriptWhenAlbumArtistIsNotEqualToArtistInOneOfTheDuplicates(logFile, deletionScript, orderedDuplicates);

                foreach (var d in orderedDuplicates.Where(d => d.SizeInBytes < orderedDuplicates.First().SizeInBytes))
                {
                    d.PrintMp3Info(logFile, "file size");
                    PrintDeletionScript(deletionScript, d.FilePath);
                }

                var leftOvers = orderedDuplicates.Where(d => d.SizeInBytes == orderedDuplicates.First().SizeInBytes);
                foreach (var d in leftOvers.Skip(1))
                {
                    d.PrintMp3Info(logFile, "no reason");
                    PrintDeletionScript(deletionScript, d.FilePath);
                }
            }
        }
    }

    /// <summary>
    /// This function takes as input songs that are grouped by their normalized title and are duplicate candidates.
    /// This function removes those candidates from the list that have different amazon id. Those are considered different songs.
    /// </summary>
    /// <param name="duplicateSongs">Songs that are grouped by their normalized title and are duplicate candidates.</param>
    /// <returns>Songs that are grouped by their normalized title and are duplicates that differ with respect to their amazon id if they have one.</returns>
    public static IDictionary<string, SongLocations> DuplicatesWithDifferentAmazonIdsAreNotDuplicates(IDictionary<string, SongLocations> duplicateSongs)
    {
        var duplicateSongsCleaned = new Dictionary<string, SongLocations>();
        foreach (var duplicateSong in duplicateSongs)
        {
            var distinctAmazonIds = duplicateSong.Value.Mp3Infos
                .Select(ds => ds.AmazonId)
                .Distinct();
            var songLocations = new List<Mp3Info>();
            foreach (var amazonId in distinctAmazonIds)
            {
                var songsByAmazonId = duplicateSong.Value.Mp3Infos
                    .Where(mp3 => mp3.AmazonId == amazonId);
                if (songsByAmazonId.Count() > 1)
                {
                    songLocations.AddRange(songsByAmazonId.ToList());
                }
            }
            var normalizedTitle = duplicateSong.Key;
            if (songLocations.Count > 1)
            {
                duplicateSongsCleaned.Add(normalizedTitle, new SongLocations(duplicateSong.Value.NormalizedTitle, songLocations));
            }
        }

        return duplicateSongsCleaned;
    }

    private static IOrderedEnumerable<Mp3Info> AddDuplicateToScriptWhenHasNoAmazonIdAndOthersHaveAmazonId(FileInfo? output, FileInfo? deletionScript, IOrderedEnumerable<Mp3Info> orderedDuplicates)
    {
        try
        {
            var ordDuplicates = orderedDuplicates;
            if (orderedDuplicates.ToList().IsAmazonIdAnDiscrimator())
            {
                var noAmazonId = orderedDuplicates.Where(mp3 => string.IsNullOrEmpty(mp3.AmazonId)).OrderByDescending(d => d.SizeInBytes);
                foreach (var d in noAmazonId)
                {
                    d.PrintMp3Info(output, "No AmazonId");
                    PrintDeletionScript(deletionScript, d.FilePath);
                }
                orderedDuplicates = orderedDuplicates.Where(mp3 => !string.IsNullOrEmpty(mp3.AmazonId)).OrderByDescending(d => d.SizeInBytes);
            }
            return orderedDuplicates;
        }
        catch (Exception ex)
        {
            Logger.WriteLine(output, ex.Message + "\n" + ex.StackTrace);
        }
        return new List<Mp3Info>().OrderByDescending(d => d.SizeInBytes);
    }

    private static IOrderedEnumerable<Mp3Info> AddDuplicateToScriptWhenAlbumArtistIsNotEqualToArtistInOneOfTheDuplicates(FileInfo? output, FileInfo? deletionScript, IOrderedEnumerable<Mp3Info> orderedDuplicates)
    {
        try
        {
            var ordDuplicates = orderedDuplicates;
            var isAlbumArtistDiscriminator = orderedDuplicates.ToList().IsAlbumArtistAnDiscrimator();
            if (isAlbumArtistDiscriminator.IsDiscriminator)
            {
                var albumArtistMismatch = orderedDuplicates.Where(mp3 => mp3.AlbumArtists.ArrayToString() != isAlbumArtistDiscriminator.Interpret).OrderByDescending(d => d.SizeInBytes);
                foreach (var d in albumArtistMismatch.Where(d => d.AlbumArtists.ArrayToString() != ordDuplicates.First().Interpret.ArrayToString()))
                {
                    d.PrintMp3Info(output, "Album Artist <> Interpret");
                    PrintDeletionScript(deletionScript, d.FilePath);
                }
                orderedDuplicates = orderedDuplicates.Where(mp3 => mp3.AlbumArtists.ArrayToString() == isAlbumArtistDiscriminator.Interpret).OrderByDescending(d => d.SizeInBytes);
            }
            return orderedDuplicates;
        }
        catch (Exception ex)
        {
            Logger.WriteLine(output, ex.Message + "\n" + ex.StackTrace);
        }
        return new List<Mp3Info>().OrderByDescending(d => d.SizeInBytes);
    }

    private static void PrintDeletionScript(FileInfo? deletionScript, string? filePath)
    {
        var escapedFilePath = filePath.EscapeSpecialChars();
        Logger.WriteLine(deletionScript, $"rm -f {escapedFilePath}");
    }
}

