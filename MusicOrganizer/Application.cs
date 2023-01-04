using MusicOrganizer.Extensions;
using MusicOrganizer.Models;
using MusicOrganizer.Services;

namespace MusicOrganizer
{
    public static class Application
    {
        public static void Run(Options options)
        {
            var appOptions = options.ToAppOptions();
            appOptions.MusicDirectories.Print(appOptions.LogFile);
            Logger.WriteLine(appOptions.LogFile, $"{DateTime.Now} Starting");
            appOptions.DeleteEmptySubDirsInMainDir();
            appOptions.DeleteEmptySubDirsInPlaylistDir();

            var mainDirScanResult = appOptions
                .ResumeOrEnumerateMp3sInMainDir()
                .ResumeOrScanMp3Tags(appOptions.ResumeMainTags!, appOptions.LogFile!)!
                .CreateDeletionScript(appOptions.DeletionScript, appOptions.LogFile);

            if (mainDirScanResult.FoundDuplicates)
            {
                Logger.WriteLine(appOptions.LogFile, $"{DateTime.Now} Aborting as there are duplicate mp3 songs.");
                return;
            }

            Logger.WriteLine(appOptions.LogFile, $"{DateTime.Now} Duplicate detection finished in main directory");

            var playListScanResult = appOptions
                .ResumeOrEnumerateMp3sInPlaylistDir()
                .ResumeOrScanMp3Tags(appOptions.ResumePlaylistTags!, appOptions.LogFile!)!
                .CreateDeletionScript(appOptions.DeletionScript, appOptions.LogFile);

            if (playListScanResult.FoundDuplicates)
            {
                Logger.WriteLine(appOptions.LogFile, $"{DateTime.Now} Aborting as there are duplicate mp3 songs in playlist.");
                return;
            }

            Logger.WriteLine(appOptions.LogFile, $"{DateTime.Now} Tag Scan on playlist mp3 files finished");

            var playlistSongsInMainDictionary = mainDirScanResult.FileTags?.FindMp3TwinInMainDir(playListScanResult.FileTags!,
                appOptions.MainMusicDirectory().Single().DirectoryInfo,
                appOptions.PlaylistDirectory().Single().DirectoryInfo,
                appOptions.LogFile);

            playlistSongsInMainDictionary.CreatePlaylistFile(appOptions.PlaylistFile!,
                appOptions.MainMusicDirectory().Single().DirectoryInfo, appOptions.LogFile);
        }

        public static List<FileInfo> ResumeOrScan(List<Mp3DirectoryInfo> playlistDirectory, AppOptions appOptions, FileInfo resumeFile)
        {
            var files = ResumeService.LoadResumeFilesPoint(resumeFile, appOptions.LogFile);
            if (files == null)
            {
                files = playlistDirectory.EnumerateMp3s(appOptions.LogFile);
                ResumeService.StoreResumeFilesPoint(resumeFile, files);
            }

            return files;
        }
    }
}

