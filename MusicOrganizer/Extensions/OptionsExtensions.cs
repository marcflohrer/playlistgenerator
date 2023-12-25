using MusicOrganizer.Models;

namespace MusicOrganizer.Extensions
{
    public static class OptionsExtensions
    {
        public static AppOptions ToAppOptions(this Options options)
        {
            ArgumentNullException.ThrowIfNull(options);
            var musicDirectories = options.PlaylistDirectory != null
                ? [new Mp3DirectoryInfo(new DirectoryInfo(options.PlaylistDirectory), false)]
                : new List<Mp3DirectoryInfo>();
            musicDirectories.Add(new Mp3DirectoryInfo(new DirectoryInfo(options.MusicDirectory!), true));
            musicDirectories = musicDirectories.Where(md => md.DirectoryInfo.Exists).ToList();
            var playListPath = new FileInfo($"{new DirectoryInfo(options.MusicDirectory!)}{Path.DirectorySeparatorChar}{options.PlaylistName}");
            FileInfo? csvInputPlaylistFile;
            if (options?.CsvInputPlaylistFile == null)
            {
                csvInputPlaylistFile = null;
            }
            else
            {
                csvInputPlaylistFile = (FileInfo?)new FileInfo(Path.Combine(options.MusicDirectory!, options.CsvInputPlaylistFile));
                if (!csvInputPlaylistFile!.Exists)
                {
                    throw new InvalidDataException($"CSV file does not exist {csvInputPlaylistFile.FullName}");
                }
                options.CsvInputPlaylistFile = csvInputPlaylistFile.FullName;
            }
            var appOptions = new AppOptions(musicDirectories,
                new FileInfo(Path.Combine(options!.PlaylistDirectory!, options.LogFile!)),
                new FileInfo(Path.Combine(options.MusicDirectory!, options.ResumeFiles!)),
                new FileInfo(Path.Combine(options.MusicDirectory!, options.ResumeTags!)),
                new FileInfo(Path.Combine(options.PlaylistDirectory!, options.ResumeFiles!)),
                new FileInfo(Path.Combine(options.PlaylistDirectory!, options.ResumeTags!)),
                options.DeletionScript != null
                    ? new FileInfo(Path.Combine(options.MusicDirectory!, options.DeletionScript))
                    : null,
                options.PlaylistName != null
                    ? playListPath
                    : null,
                    csvInputPlaylistFile
                );

            if (appOptions.MainMusicDirectory().Single().DirectoryInfo.FullName
                == appOptions.PlaylistDirectory().Single().DirectoryInfo.FullName)
            {
                throw new InvalidDataException("Main directory and playlist directory must not be the same");
            }

            return appOptions;
        }
    }
}

