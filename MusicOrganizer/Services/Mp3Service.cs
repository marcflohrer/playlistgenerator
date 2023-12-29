using MusicOrganizer.Models;

namespace MusicOrganizer.Services;

public static class Mp3Service
{
    public static List<FileInfo> EnumerateMp3s(this Mp3DirectoryInfo mp3Directories, FileInfo? output)
    {
        var mp3Files = new List<FileInfo>();
        var directories = new List<DirectoryInfo>
        {
            mp3Directories.DirectoryInfo
        };

        foreach (var (subdir, files) in Directories(directories))
        {
            files.AddRange(subdir.SelectMany(sd => sd.EnumerateMp3s(output)));
            IEnumerable<(FileInfo f, TagLib.File mp3Tags)> enumerable()
            {
                foreach (var f in files)
                {
                    if (f.FullName.EndsWith(".mp3") && !f.Name.StartsWith("._"))
                    {
                        var mp3Tags = f.ParseMp3Tags();
                        if (mp3Tags != null)
                        {
                            yield return (f, mp3Tags);
                        }
                    }
                }
            }

            foreach (var (f, mp3Tags) in enumerable())
            {
                if (!mp3Tags.PossiblyCorrupt)
                {
                    var sonosService = new SonosService(new FileService());
                    var cleanedFileName = sonosService.ToAsciiOnly(f, output);
                    mp3Files.Add(cleanedFileName);
                }
                else if (f.FullName.EndsWith(".mp3"))
                {
                    Logger.WriteLine(output, $"{DateTime.Now} Corrupt mp3 file found: {f.FullName}");
                }
            }
        }

        return mp3Files;
    }

    private static IEnumerable<(IEnumerable<Mp3DirectoryInfo> subdir, List<FileInfo> files)> Directories(List<DirectoryInfo>? directories)
    {
        if (directories == null)
        {
            throw new InvalidOperationException("Mp3TagService.Directories: Parameter must not be null.");
        }
        foreach (var dir in directories)
        {
            var subdir = dir.EnumerateDirectories().Select(sd => new Mp3DirectoryInfo(sd));
            var files = dir.EnumerateFiles().ToList();
            yield return (subdir, files);
        }
    }

}

