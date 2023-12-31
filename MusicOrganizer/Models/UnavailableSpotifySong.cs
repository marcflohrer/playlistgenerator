namespace MusicOrganizer.Models;

public class UnavailableSpotifySong

{
    public string TrackName { get; set; } = "";
    public string AlbumName { get; set; } = "";
    public string ArtistNames { get; set; } = "";
    public string DurationMs { get; set; } = "";
    public string AddedBy { get; set; } = "";
    public string AddedAt { get; set; } = "";


    public static UnavailableSpotifySong FromCsvColumns(List<string> columns)
    {
        var trackName = columns[2];
        var albumName = columns[3];
        var artistNames = columns[4];
        var durationMs = columns[6];
        var addedBy = columns[8];
        var addedAt = columns[9];
        return new UnavailableSpotifySong()
        {
            TrackName = trackName,
            AlbumName = albumName,
            ArtistNames = artistNames,
            DurationMs = durationMs,
            AddedBy = addedBy,
            AddedAt = addedAt
        };
    }

    public ExportifyPlaylistEntry ToPlaylistEntry() => new()
    {
        SpotifyId = "-",
        ArtistIds = "-",
        TrackName = this.TrackName,
        AlbumName = this.AlbumName,
        ArtistNames = this.ArtistNames,
        ReleaseDate = "1970-01-01",
        DurationMs = this.DurationMs,
        Popularity = "-1",
        AddedBy = this.AddedBy,
        AddedAt = this.AddedAt,
        Genres = "None",
        Danceability = "-1",
        Energy = "-1",
        Key = "-1",
        Loudness = "-1",
        Mode = "-1",
        Speechiness = "-1",
        Acousticness = "-1",
        Instrumentalness = "-1",
        Liveness = "-1",
        Valence = "-1",
        Tempo = "-1",
        TimeSignature = "-1",
    };
}
