using System.Text;
using System.Text.Json;
using MusicOrganizer.Models;
using MusicOrganizer.Services;

namespace MusicOrganizer.Extensions;

public static class CreatePlaylistExtensions
{
    public static void CreatePlaylistFile(this List<SongLocation>? playlistMp3Files,
        FileInfo playlist, DirectoryInfo mainDir, FileInfo? output)
    {
        try
        {

            if (playlist.Exists)
            {
                playlist.Delete();
            }
            var playlistContent = new StringBuilder();
            playlistContent.AppendLine("#EXTM3U");
            playlistContent.AppendLine("#EXTENC: UTF-8");
            playlistContent.AppendLine($"#PLAYLIST: {playlist.Name.Replace(".m3u8", string.Empty).Replace(".m3u", string.Empty)}");
            foreach (var playlistMp3File in playlistMp3Files!)
            {
                var commentOut = string.Empty;
                if (playlistMp3File.Mp3Info.IsMissing())
                {
                    commentOut = "#";
                }
                playlistContent.AppendLine($"{commentOut}#EXTINF:{playlistMp3File?.Mp3Info?.DurationSeconds},{string.Join('&', playlistMp3File?.Mp3Info?.Interpret ?? [])} - {playlistMp3File?.Mp3Info?.Title ?? string.Empty}");
                if (playlistMp3File?.Mp3Info?.FilePath == null)
                {
                    Logger.WriteLine(output, $"Path is null. {JsonSerializer.Serialize(playlistMp3File?.Mp3Info)}");
                }
                else
                {
                    if (!playlistMp3File.Mp3Info.IsMissing())
                    {
                        var playListEntry = PlaylistService.CreatePlaylistEntry(mainDir, new FileInfo(playlistMp3File.Mp3Info.FilePath), output);
                        if (!string.IsNullOrWhiteSpace(playListEntry))
                        {
                            playlistContent.AppendLine($"{playListEntry}");
                        }
                    }
                    else
                    {
                        playlistContent.AppendLine($"{commentOut}{playlistMp3File.Mp3Info.Interpret.FirstOrDefault()}/{playlistMp3File.Mp3Info.Title}.mp3");
                    }
                }
            }
            Logger.WriteLine(playlist, playlistContent.ToString());
        }
        catch (Exception ex)
        {
            Logger.WriteLine(output, $"{ex.Message};{ex.StackTrace}");
            throw;
        }
    }

    public static void PrintMp3Info(this Mp3Info? d, FileInfo? output, string deleteReason)
    {
        Logger.WriteLine(output, $"{DateTime.Now} \t --> T: {d?.Title}; I: {string.Join(';', d?.Interpret ?? Array.Empty<string>())}; Reason: {deleteReason}; {d?.FilePath}; {d?.Md5Hash}; {d?.Number}; {string.Join(';', d?.AlbumArtists ?? Array.Empty<string>())}; Bytes : {d?.SizeInBytes}; AmazonId : {d?.AmazonId}; Year : {d?.Year}");
    }
}

