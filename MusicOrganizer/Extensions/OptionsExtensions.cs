using MusicOrganizer.Models;

namespace MusicOrganizer.Extensions
{
    public static class OptionsExtensions
    {
        public static AppOptions ToAppOptions(this Options options)
        {
            ArgumentNullException.ThrowIfNull(options);
            var musicDirectory = new Mp3DirectoryInfo(new DirectoryInfo(options.MusicDirectory!));
            var csvPlaylistDirectory = new DirectoryInfo(options.CsvPlaylistDirectory!);
            var csvInputPlaylistFiles = new List<FileInfo>();
            foreach (var csvFile in csvPlaylistDirectory.GetFiles("*.csv").Where(f => f.Extension == ".csv"))
            {
                if (!csvFile.Name.StartsWith('.'))
                {
                    csvInputPlaylistFiles.Add(csvFile);
                }
            }

            var appOptions = new AppOptions(
                musicDirectory,
                new FileInfo(Path.Combine(options!.MusicDirectory!, "logger.txt")),
                new FileInfo(Path.Combine(options.MusicDirectory!, "resumefiles.txt")),
                new FileInfo(Path.Combine(options.MusicDirectory!, "resumetags.txt")),
                new FileInfo(Path.Combine(options.MusicDirectory!, "deletion.sh")),
                csvInputPlaylistFiles
                );

            return appOptions;
        }
    }
}

