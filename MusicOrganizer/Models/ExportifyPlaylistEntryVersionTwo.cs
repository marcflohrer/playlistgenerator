using CsvHelper.Configuration.Attributes;
using MusicOrganizer.Extensions;

namespace MusicOrganizer.Models;

public class ExportifyPlaylistEntryVersionTwo
{
    [Index(1)]
    [Name("Track Name")]
    public string TrackName { get; set; } = "";

    [Index(2)]
    [Name("Album Name")]
    public string AlbumName { get; set; } = "";

    [Index(3)]
    [Name("Artist Name(s)")]
    public string ArtistNames { get; set; } = "";

    [Index(4)]
    [Name("Release Date")]
    public string ReleaseDate { get; set; } = "";

    [Index(5)]
    [Name("Duration (ms)")]

    public string DurationMs { get; set; } = "";

    [Index(6)]
    [Name("Popularity")]
    public string Popularity { get; set; } = "";

    [Index(7)]
    [Name("Added By")]
    public string AddedBy { get; set; } = "";

    [Index(8)]
    [Name("Added At")]
    public string AddedAt { get; set; } = "";

    [Index(9)]
    [Name("Genres")]
    public string Genres { get; set; } = "";

    [Index(10)]
    [Name("Danceability")]
    public string Danceability { get; set; } = "";

    [Index(11)]
    [Name("Energy")]
    public string Energy { get; set; } = "";

    [Index(12)]
    [Name("Key")]
    public string Key { get; set; } = "";

    [Index(13)]
    [Name("Loudness")]
    public string Loudness { get; set; } = "";

    [Index(14)]
    [Name("Mode")]
    public string Mode { get; set; } = "";

    [Index(15)]
    [Name("Speechiness")]
    public string Speechiness { get; set; } = "";

    [Index(16)]
    [Name("Acousticness")]
    public string Acousticness { get; set; } = "";

    [Index(17)]
    [Name("Instrumentalness")]
    public string Instrumentalness { get; set; } = "";

    [Index(18)]
    [Name("Liveness")]
    public string Liveness { get; set; } = "";

    [Index(19)]
    [Name("Valence")]
    public string Valence { get; set; } = "";

    [Index(20)]
    [Name("Tempo")]
    public string Tempo { get; set; } = "";

    [Index(20)]
    [Name("Time Signature")]
    public string TimeSignature { get; set; } = "";

    public Mp3Info ToPlaylistEntry(IList<MusicBrainzTagMap> tagMaps)
    {
        var releaseYear = 0;
        if (ReleaseDate.Length == 4)
        {
            releaseYear = int.Parse(ReleaseDate);
        }
        else if (ReleaseDate.Split("-").Length == 3)
        {
            releaseYear = DateTime.Parse(ReleaseDate).Year;
        }
        var fixedArtistNames = StringExtensions.ReplaceSpotifyTagMismatches(ArtistNames, [.. tagMaps]);
        var artists = GetArtistList(fixedArtistNames);
        return new Mp3Info(
            string.Empty,
            0,
            artists.ToArray()[0],
            artists,
            artists,
            TrackName,
            -1,
            string.Empty,
            releaseYear,
            GetDurationInSeconds());
    }

    private static string[] GetArtistList(string artistNames)
        => artistNames.Split(",").Select(an => an.Split('\u0026')).SelectMany(an => an).Select(an => an.Trim()).ToArray();

    private int GetDurationInSeconds()
    {
        return int.Parse(DurationMs) / 1000;
    }

    public string ToCsvString()
    {
        return $"{TrackName},{AlbumName},{ArtistNames},{ReleaseDate},{DurationMs},{Popularity},{AddedBy},{AddedAt},{Genres},{Danceability},{Energy},{Key},{Loudness},{Mode},{Speechiness},{Acousticness},{Instrumentalness},{Liveness},{Valence},{Tempo},{TimeSignature}";
    }
}
