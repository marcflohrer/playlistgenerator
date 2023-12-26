using MusicOrganizer.Extensions;
using MusicOrganizer.Models;
using MusicOrganizer.Services;
using static MusicOrganizer.Services.Mp3TagService;

namespace MusicOrganizer
{
    public static class Application
    {
        public static void Run(Options options)
        {
            var appOptions = options.ToAppOptions();
            Logger.WriteLine(appOptions.LogFile, $"-> {appOptions.MusicDirectory.DirectoryInfo.FullName}");
            Logger.WriteLine(appOptions.LogFile, $"{DateTime.Now} Starting to create m3u playlists.");
            appOptions.DeleteEmptySubDirsInMusicDir();

            appOptions
                .ResumeOrEnumerateMp3sInMainDir()
                .ResumeOrScanMp3Tags(appOptions.ResumeMainTags!, appOptions.LogFile!)!
                .CreateDeletionScriptForDuplicates(appOptions.DeletionScript, appOptions.LogFile)
                .CreateM3uPlaylists(appOptions);
        }
    }
}

