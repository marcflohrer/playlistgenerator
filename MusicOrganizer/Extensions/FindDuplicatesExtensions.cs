using MusicOrganizer.Models;
using MusicOrganizer.Services;

namespace MusicOrganizer.Extensions
{
    public static class FindDuplicatesExtensions
    {

        public static List<Mp3TagService.FileTags>? ResumeOrScanMp3Tags(this List<FileInfo>? mp3Files, FileInfo resumeTags, FileInfo logFile)
        {
            var fileTags = ResumeService.LoadResumeTagsPoint(resumeTags) ?? [];
            if (fileTags.Count == 0)
            {
                fileTags = mp3Files?.GetMp3Info(logFile);
                ResumeService.StoreResumeTagsPoint(resumeTags, fileTags ?? []);
            }
            Logger.WriteLine(logFile, $"{DateTime.Now} Tag Scan on mp3 files in main directory finished");
            return fileTags;
        }

        public static void Print(this List<Mp3DirectoryInfo> mp3DirectoryInfos, FileInfo? output)
        {
            if (mp3DirectoryInfos.Count == 0)
            {
                Logger.WriteLine(output, $"{DateTime.Now} No existing directories entered. Exiting.");
                return;
            }
            else
            {
                foreach (var md in mp3DirectoryInfos)
                {
                    var isMainDir = md.IsMainDir ? " (main dir)" : string.Empty;
                    Logger.WriteLine(output, $"-> {md.DirectoryInfo.FullName}{isMainDir}");
                }
            }
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

                    orderedDuplicates = RemoveWhereAmazonIdIsNotAvailableInOneOfTheDuplicates(logFile, deletionScript, orderedDuplicates);

                    orderedDuplicates = RemoveWhereAlbumArtistIsNotEqualToArtistInOneOfTheDuplicates(logFile, deletionScript, orderedDuplicates);

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

        private static IOrderedEnumerable<Mp3Info> RemoveWhereAmazonIdIsNotAvailableInOneOfTheDuplicates(FileInfo? output, FileInfo? deletionScript, IOrderedEnumerable<Mp3Info> orderedDuplicates)
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

        private static IOrderedEnumerable<Mp3Info> RemoveWhereAlbumArtistIsNotEqualToArtistInOneOfTheDuplicates(FileInfo? output, FileInfo? deletionScript, IOrderedEnumerable<Mp3Info> orderedDuplicates)
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
            filePath = filePath.EscapeSpecialChars();
            Logger.WriteLine(deletionScript, $"rm -f {filePath}");
        }
    }
}

