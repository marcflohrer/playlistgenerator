using System;
using MusicOrganizer.Models;
using MusicOrganizer.Services;

namespace MusicOrganizer.Extensions;

public static class AppOptionsExtensions
{
    public static List<Mp3DirectoryInfo> MainMusicDirectory(this AppOptions appOptions)
        => appOptions.MusicDirectories.Where(pd => pd.isMainDir).ToList();
    public static List<Mp3DirectoryInfo> PlaylistDirectory(this AppOptions appOptions)
        => appOptions.MusicDirectories.Where(pd => pd.isMainDir == false).ToList();

    public static void DeleteEmptySubDirsInMainDir(this AppOptions appOptions)
    {
        DeleteEmptySubDirectories(appOptions, appOptions.MainMusicDirectory()
            .Single()
            .DirectoryInfo);
    }

    public static void DeleteEmptySubDirsInPlaylistDir(this AppOptions appOptions)
    {
        DeleteEmptySubDirectories(appOptions, appOptions.PlaylistDirectory()
            .Single()
            .DirectoryInfo);
    }

    private static void DeleteEmptySubDirectories(AppOptions appOptions, DirectoryInfo directoryInfo)
    {
        var countDeletedEmptyDirs = appOptions.MusicDirectories.Select(md => directoryInfo.DeleteEmptySubDirectories()).Sum();
        if (countDeletedEmptyDirs > 0)
        {
            var directoryNames = countDeletedEmptyDirs != 1 ? "directories" : "directory";
            Logger.WriteLine(appOptions.LogFile, $"{DateTime.Now} Deleted {countDeletedEmptyDirs} empty {directoryNames}");
        }
    }

    public static List<FileInfo> ResumeOrEnumerateMp3sInMainDir(this AppOptions appOptions)
    {
        return appOptions.ResumeOrEnumerateMp3s(appOptions.MainMusicDirectory(), appOptions.ResumeMainFiles);
    }

    public static List<FileInfo> ResumeOrEnumerateMp3sInPlaylistDir(this AppOptions appOptions)
    {
        return appOptions.ResumeOrEnumerateMp3s(appOptions.PlaylistDirectory(), appOptions.ResumePlaylistFiles);
    }

    private static List<FileInfo> ResumeOrEnumerateMp3s(this AppOptions appOptions, List<Mp3DirectoryInfo> mp3DirectoryInfos, FileInfo resumeFile)
    {
        var result = mp3DirectoryInfos
            .ResumeOrEnumerateMp3s(appOptions.LogFile!, resumeFile);
        Logger.WriteLine(appOptions.LogFile, $"{DateTime.Now} Found {result.Count} mp3 files in main directory");
        return result;
    }
}

