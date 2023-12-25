using System.Security.Cryptography;
using MusicOrganizer.Extensions;
using MusicOrganizer.Models;
using MusicOrganizer.Services;
using TagLib;

namespace MusicOrganizer;

public static class FileInfoExtensions
{
    public static string Md5(this FileInfo fileInfo)
    {
        using (var md5 = MD5.Create())
        {
            using var stream = System.IO.File.OpenRead(fileInfo.FullName);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
        throw new Exception();
    }

    public static Mp3Info GetMp3Info(this FileInfo f, FileInfo? output)
    {
        var tagLibFile = TagLib.File.Create(f.FullName);
        var duration = (int)tagLibFile.Properties.Duration.TotalSeconds;
        var tags = tagLibFile.GetTag(TagTypes.Id3v2);
        if (tags.IsEmpty)
        {
            tags = tagLibFile.GetTag(TagTypes.Id3v1);
            if (tags.IsEmpty)
            {
                Logger.WriteLine(output, $"{DateTime.Now} Mp3 without tags found: {f.FullName}");
            }
            else
            {
                tags.CleanTags(output);
            }
        }
        var mp3Info = new Mp3Info(f.FullName,
                           f.Md5(),
                           f.Length,
                           tags.Performers,
                           tags.AlbumArtists,
                           tags.Title,
                           tags.Title.NormalizeSongTag(NormalizeMode.Loose),
                           (int)tags.Track,
                           tags.AmazonId,
                           (int)tags.Year,
                           duration);
        return mp3Info;
    }
}

