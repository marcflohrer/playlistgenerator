using MusicOrganizer.Models;
using MusicOrganizer.Services;
using static MusicOrganizer.Services.Mp3TagService;

namespace MusicOrganizer.Extensions;

public static class FindDuplicatesExtensions
{

    /// <summary>
    /// Resume Or Scan Mp3 Tags.
    /// </summary>
    /// <param name="mp3Files">Mp3 Files.</param>
    /// <param name="resumeTags">File that contains the tags of the most recent scan.</param>
    /// <param name="tagMaps">Maps tags in the playlist to the tags of the local music library.</param>
    /// <param name="logFile">Log file.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Create deletion script for duplicate songs.
    /// </summary>
    /// <param name="duplicatesongs">Duplicate songs.</param>
    /// <param name="logFile">Log File.</param>
    /// <param name="deletionScript">Deletion Script.</param>
    public static void CreateDeletionScript(this Dictionary<string, SongLocations> duplicatesongs, FileInfo? logFile, FileInfo? deletionScript)
    {
        foreach (var song in duplicatesongs)
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
            var songLocations = duplicateSong.Value.Mp3Infos
                .GroupBy(mp3 => mp3.AmazonId)
                .Where(group => group.Count() > 1)
                .SelectMany(group => group)
                .ToList();

            if (songLocations.Count > 1)
            {
                duplicateSongsCleaned.Add(duplicateSong.Key, new SongLocations(duplicateSong.Value.NormalizedTitle, songLocations));
            }
        }

        return duplicateSongsCleaned;
    }

    private static IOrderedEnumerable<Mp3Info> AddDuplicateToScriptWhenHasNoAmazonIdAndOthersHaveAmazonId(FileInfo? output, FileInfo? deletionScript, IOrderedEnumerable<Mp3Info> orderedDuplicates)
    {
        try
        {
            if (orderedDuplicates.Any(mp3 => !string.IsNullOrEmpty(mp3.AmazonId)))
            {
                var noAmazonId = orderedDuplicates.Where(mp3 => string.IsNullOrEmpty(mp3.AmazonId));
                ProcessDuplicates(noAmazonId, output, deletionScript, "No AmazonId");

                orderedDuplicates = orderedDuplicates.Where(mp3 => !string.IsNullOrEmpty(mp3.AmazonId)).OrderByDescending(o => o.SizeInBytes);
            }
            return orderedDuplicates.OrderByDescending(d => d.SizeInBytes);
        }
        catch (Exception ex)
        {
            Logger.WriteLine(output, ex.Message + "\n" + ex.StackTrace);
            return Enumerable.Empty<Mp3Info>().OrderByDescending(d => d.SizeInBytes);
        }
    }

    private static void ProcessDuplicates(IEnumerable<Mp3Info> duplicates, FileInfo? output, FileInfo? deletionScript, string reason)
    {
        foreach (var d in duplicates)
        {
            d.PrintMp3Info(output, reason);
            PrintDeletionScript(deletionScript, d.FilePath);
        }
    }

    private static IOrderedEnumerable<Mp3Info> AddDuplicateToScriptWhenAlbumArtistIsNotEqualToArtistInOneOfTheDuplicates(FileInfo? output, FileInfo? deletionScript, IOrderedEnumerable<Mp3Info> orderedDuplicates)
    {
        try
        {
            var isAlbumArtistDiscriminator = orderedDuplicates.ToList().IsAlbumArtistAnDiscrimator();
            if (isAlbumArtistDiscriminator.IsDiscriminator)
            {
                var albumArtistMismatch = orderedDuplicates.Where(mp3 => mp3.AlbumArtists.ArrayToString() != isAlbumArtistDiscriminator.Interpret);
                ProcessDuplicates(albumArtistMismatch, output, deletionScript, "Album Artist <> Interpret");

                orderedDuplicates = orderedDuplicates.Where(mp3 => mp3.AlbumArtists.ArrayToString() == isAlbumArtistDiscriminator.Interpret).OrderByDescending(o => o.SizeInBytes);
            }
            return orderedDuplicates.OrderByDescending(d => d.SizeInBytes);
        }
        catch (Exception ex)
        {
            Logger.WriteLine(output, ex.Message + "\n" + ex.StackTrace);
            return Enumerable.Empty<Mp3Info>().OrderByDescending(d => d.SizeInBytes);
        }
    }

    private static void PrintDeletionScript(FileInfo? deletionScript, string? filePath)
    {
        var escapedFilePath = filePath.EscapeSpecialChars();
        Logger.WriteLine(deletionScript, $"rm -f {escapedFilePath}");
    }
}
