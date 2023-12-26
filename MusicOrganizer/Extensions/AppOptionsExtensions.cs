using MusicOrganizer.Models;
using MusicOrganizer.Services;

namespace MusicOrganizer.Extensions;

public static class AppOptionsExtensions
{

    public static void DeleteEmptySubDirsInMusicDir(this AppOptions appOptions)
    {
        DeleteEmptySubDirectories(appOptions);
    }

    private static void DeleteEmptySubDirectories(AppOptions appOptions)
    {
        var countDeletedEmptyDirs = appOptions.MusicDirectory.DirectoryInfo.DeleteEmptySubDirectories();
        if (countDeletedEmptyDirs > 0)
        {
            var directoryNames = countDeletedEmptyDirs != 1 ? "directories" : "directory";
            Logger.WriteLine(appOptions.LogFile, $"{DateTime.Now} Deleted {countDeletedEmptyDirs} empty {directoryNames}");
        }
    }

    public static List<FileInfo> ResumeOrEnumerateMp3sInMainDir(this AppOptions appOptions)
    {
        return appOptions.ResumeOrEnumerateMp3s(appOptions.MusicDirectory, appOptions.ResumeMainFiles);
    }

    private static List<FileInfo> ResumeOrEnumerateMp3s(this AppOptions appOptions, Mp3DirectoryInfo mp3DirectoryInfos, FileInfo resumeFile)
    {
        var result = mp3DirectoryInfos
            .ResumeOrEnumerateMp3s(appOptions.LogFile!, resumeFile);
        Logger.WriteLine(appOptions.LogFile, $"{DateTime.Now} Found {result.Count} mp3 files in main directory");
        return result;
    }
}

