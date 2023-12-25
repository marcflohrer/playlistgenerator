using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MusicOrganizer.Models;
namespace MusicOrganizer.Services;

public static class CsvToMp3InfoParser
{
    public static IList<PlaylistEntry> ToMp3InfoList(this FileInfo csvFilePlayList)
    {
        ArgumentNullException.ThrowIfNull(csvFilePlayList);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = Environment.NewLine,
        };
        using var reader = new StreamReader(path: csvFilePlayList.FullName);
        using var csv = new CsvReader(reader, config, false);
        var exportifyPlaylist = csv.GetRecords<ExportifyPlaylist>();
        var playlistEntries = new List<PlaylistEntry>();
        foreach (var playlistEntry in exportifyPlaylist)
        {
            playlistEntries.Add(playlistEntry.ToPlaylistEntry());
        }
        return playlistEntries;
    }
}
