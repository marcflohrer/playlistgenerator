using System.Security.Cryptography;
using MusicOrganizer.Extensions;
using MusicOrganizer.Models;
using MusicOrganizer.Services;
using TagLib;

namespace MusicOrganizer;

public static class FileInfoExtensions
{
    public static Mp3Info GetMp3Info(this FileInfo f, IList<MusicBrainzTagMap> tagMaps, FileInfo? output)
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
        var mainArtist = Mp3TagService.NormalizeSongTag(tags.Performers[0], tagMaps, NormalizeMode.StrictInterpret);
        var mp3Info = new Mp3Info(
            f.FullName,
            f.Length,
            mainArtist,
            tags.Performers,
            tags.AlbumArtists,
            tags.Title,
            (int)tags.Track,
            tags.AmazonId,
            (int)tags.Year,
            duration);
        return mp3Info;
    }
}

