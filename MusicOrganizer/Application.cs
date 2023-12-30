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

            var tagMaps = AppSettingsProvider.GetTagMaps();
            Logger.WriteLine(appOptions.LogFile, $"-> {appOptions.MusicDirectory.DirectoryInfo.FullName}");
            Logger.WriteLine(appOptions.LogFile, $"{DateTime.Now} Starting to create m3u playlists.");
            appOptions.DeleteEmptySubDirsInMusicDir();

            appOptions
                .ResumeOrEnumerateMp3sInMainDir()
                .ResumeOrScanMp3Tags(appOptions.ResumeMainTags!, tagMaps, appOptions.LogFile!)!
                .CreateDeletionScriptForDuplicates(tagMaps, appOptions.DeletionScript, appOptions.LogFile)
                .CreateM3uPlaylists(tagMaps, appOptions);
            Logger.WriteLine(appOptions.LogFile, $"{DateTime.Now} Done.");
        }
    }
}
