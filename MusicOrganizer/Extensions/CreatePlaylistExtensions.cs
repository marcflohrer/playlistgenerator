using System;
using System.IO;
using System.Text.Json;
using System.Web;
using MusicOrganizer.Models;
using MusicOrganizer.Services;

namespace MusicOrganizer.Extensions
{
    public static class CreatePlaylistExtensions
    {
        public static void PrintPlaylist(this List<Mp3DirectoryInfo> mp3DirectoryInfos, FileInfo? output)
        {
            if (!mp3DirectoryInfos.Any())
            {
                Logger.WriteLine(output, $"{DateTime.Now} No existing directories entered. Exiting.");
                return;
            }
            else
            {
                foreach (var md in mp3DirectoryInfos)
                {
                    var isMainDir = md.isMainDir ? " (main dir)" : string.Empty;
                    Logger.WriteLine(output, $"-> {md.DirectoryInfo.FullName}{isMainDir}");
                }
            }
        }

        public static void CreatePlaylistFile(this List<SongLocation>? playlistMp3Files,
            FileInfo playlist, DirectoryInfo mainDir, FileInfo? output)
        {
            try
            {

                if (playlist.Exists)
                {
                    playlist.Delete();
                }
                Logger.WriteLine(playlist, "#EXTM3U");
                Logger.WriteLine(playlist, "#EXTENC: UTF-8");
                Logger.WriteLine(playlist, $"#PLAYLIST: {playlist.Name.Replace(".m3u8", string.Empty).Replace(".m3u", string.Empty)}");
                foreach (var playlistMp3File in playlistMp3Files!)
                {
                    Logger.WriteLine(playlist, $"#EXTINF:{playlistMp3File?.Mp3Info?.DurationSeconds},{string.Join('&', playlistMp3File?.Mp3Info?.Interpret ?? Array.Empty<string>())} - {playlistMp3File?.Mp3Info?.Title ?? string.Empty}");
                    if (playlistMp3File?.Mp3Info?.FilePath == null)
                    {
                        Logger.WriteLine(output, $"Path is null. {JsonSerializer.Serialize(playlistMp3File?.Mp3Info)}");
                    }
                    else
                    {
                        var playListEntry = PlaylistService.CreatePlaylistEntry(mainDir, new FileInfo(playlistMp3File.Mp3Info.FilePath), output);
                        if (!string.IsNullOrWhiteSpace(playListEntry))
                        {
                            Logger.WriteLine(playlist, $"{playListEntry}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(output, $"{ex.Message};{ex.StackTrace}");
            }
        }

        public static void PrintMp3Info(this Mp3Info? d, FileInfo? output, string deleteReason)
        {
            Logger.WriteLine(output, $"{DateTime.Now} \t --> T: {d?.Title}; I: {string.Join(';', d?.Interpret ?? Array.Empty<string>())}; Reason: {deleteReason}; {d?.FilePath}; {d?.Md5Hash}; {d?.Number}; {string.Join(';', d?.AlbumArtists ?? Array.Empty<string>())}; Bytes : {d?.SizeInBytes}; AmazonId : {d?.AmazonId}; Year : {d?.Year}");
        }

        private static void PrintPlaylistEntry(FileInfo? deletionScript, string? filePath)
        {
            filePath = filePath.EscapeSpecialChars();
            Logger.WriteLine(deletionScript, $"rm -f {filePath}");
        }
    }
}

