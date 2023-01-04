using MusicOrganizer.Extensions;
using MusicOrganizer.Models;

namespace MusicOrganizer.Services;

public static class Mp3TagService
{
    public record FileTags(string FilePath, Mp3Info Mp3Info);
    public static List<FileTags> GetMp3Info(this List<FileInfo> files, FileInfo? output)
    {
        var fileTags = new List<FileTags>();
        foreach (var f in files)
        {
            var mp3Info = f.GetMp3Info(output);
            if (mp3Info != null)
            {
                fileTags.Add(new(f.FullName, mp3Info));
            }
            else
            {
                Logger.WriteLine(output, $"No tag found: {f.FullName}");
            }
        }
        return fileTags;
    }

    public static List<SongLocation> FindMp3TwinInMainDir(this List<FileTags> mainDirFileTags,
        List<FileTags> playListFileTags, DirectoryInfo mainDir, DirectoryInfo playlistDir,
        FileInfo? output)
    {
        var normalizedMainDirFileTag = NormalizeFileTags(mainDirFileTags);
        ResumeService.Store(normalizedMainDirFileTag, new FileInfo(Path.Combine(mainDir.FullName, "normalizedFileTags.txt")), output);
        var playList = NormalizeFileTags(playListFileTags);
        ResumeService.Store(playList, new FileInfo(Path.Combine(playlistDir.FullName, "normalizedFileTags.txt")), output);
        var playlistSongLocations = new List<SongLocation>();
        foreach (var normalizedTitle in playList.Keys)
        {
            if (normalizedMainDirFileTag.TryGetValue(normalizedTitle, out var value))
            {
                var mp3Info = value.Mp3Infos.First();
                playlistSongLocations.Add(new SongLocation(normalizedTitle, mp3Info));
            }
            else
            {
                Logger.WriteLine(output, $"{DateTime.Now} Missing mp3 in main directory: {normalizedTitle}");
            }
        }
        return playlistSongLocations;
    }

    private static Dictionary<string, SongLocations> NormalizeFileTags(List<FileTags> mainDirFileTags)
    {
        var mp3Files = new Dictionary<string, SongLocations>();
        foreach (var f in mainDirFileTags)
        {
            var mp3Info = f.Mp3Info;
            var normalizedTitle = Normalize($"{mp3Info.Title} + {string.Join(',', mp3Info.Interpret)}");
            var file = new FileInfo(f.FilePath);
            if (!mp3Files.TryGetValue(normalizedTitle, out var songInfo))
            {
                songInfo = new SongLocations(normalizedTitle, new List<Mp3Info> { mp3Info });
            }
            else
            {
                songInfo.Mp3Infos.Add(mp3Info);
                songInfo = new SongLocations(normalizedTitle, songInfo.Mp3Infos);
                mp3Files.Remove(normalizedTitle);
            }
            mp3Files.Add(normalizedTitle, songInfo);
        }

        return mp3Files;
    }

    public static void LogDuplicates(this Dictionary<string, SongLocations> songLocations, FileInfo? logFile)
    {
        var duplicateCount = songLocations?.ToList().Where(ds => ds.Value.Mp3Infos.Count > 1).ToList().Count;
        if (duplicateCount > 0)
        {
            var fileSingularPlural = duplicateCount == 1 ? "file" : "files";
            Logger.WriteLine(logFile, $"{DateTime.Now} Found {(duplicateCount)} songs with multiple {fileSingularPlural}.");
        }
    }

    public record ScanResult(bool FoundDuplicates, List<FileTags> FileTags);

    public static ScanResult CreateDeletionScript(
        this List<FileTags> fileTags,
        FileInfo? deletionScript,
        FileInfo? output)
    {
        var mp3Files = new Dictionary<string, SongLocations>();
        foreach (var f in fileTags)
        {
            var mp3Info = f.Mp3Info;
            var normalizedTitle = Normalize($"{mp3Info.Title} + {string.Join(',', mp3Info.Interpret)}");
            var file = new FileInfo(f.FilePath);
            bool addSongInfo = false;
            if (!mp3Files.TryGetValue(normalizedTitle, out var songInfo))
            {
                songInfo = new SongLocations(normalizedTitle, new List<Mp3Info> { mp3Info });
                addSongInfo = true;
            }
            else
            {
                var filePathCandidateToAdd = mp3Info.FilePath;
                if (!songInfo.Mp3Infos.Where(mp3 => mp3.FilePath.ToLowerInvariant() == filePathCandidateToAdd.ToLowerInvariant()).Any())
                {
                    songInfo.Mp3Infos.Add(mp3Info);
                    songInfo = new SongLocations(normalizedTitle, songInfo.Mp3Infos);
                    mp3Files.Remove(normalizedTitle);
                    addSongInfo = true;
                }
                else
                {

                }
            }
            if (addSongInfo)
            {
                mp3Files.Add(normalizedTitle, songInfo);
            }
        }

        var list = mp3Files.ToList().Where(mp3 => mp3.Value.Mp3Infos.Count() > 1);
        var duplicateMp3Files = new Dictionary<string, SongLocations>();
        foreach (var kvp in list)
        {
            kvp.Value.Mp3Infos.ForEach(m => m.PrintMp3Info(output, "<not decided>"));
            duplicateMp3Files.Add(kvp.Key, kvp.Value);
        }
        duplicateMp3Files?.LogDuplicates(output);
        if (duplicateMp3Files != null && duplicateMp3Files.Any())
        {
            duplicateMp3Files?.CreateDeletionScript(output, deletionScript);
            Logger.WriteLine(output, $"{DateTime.Now} Aborting as there are duplicate mp3 songs.");
            return new ScanResult(true, fileTags);
        }
        return new ScanResult(false, fileTags);
    }

    private static string Normalize(string title)
    {
        var normalized = title.ToLowerInvariant();
        // remove punctuation marks
        var noPunctuation = normalized.RemovePunctuation();
        var noTextInBrackets = noPunctuation.RemoveContentInBrackets();
        var noUnicode = noTextInBrackets.ToM3uCompliantPath().Text;
        return noUnicode;
    }
}

