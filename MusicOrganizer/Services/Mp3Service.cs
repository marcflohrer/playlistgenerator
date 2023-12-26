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
        foreach (var (subdir, files) in from dir in directories
                                        let subdir = dir.EnumerateDirectories().Select(sd => new Mp3DirectoryInfo(sd))
                                        let files = dir.EnumerateFiles().ToList()
                                        select (subdir, files))
        {
            files.AddRange(subdir.SelectMany(sd => sd.EnumerateMp3s(output)));
            foreach (var (f, mp3Tags) in from f in files
                                         where f.FullName.EndsWith(".mp3") && !f.Name.StartsWith("._")
                                         let mp3Tags = f.ParseMp3Tags()
                                         where mp3Tags != null
                                         select (f, mp3Tags))
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

}

