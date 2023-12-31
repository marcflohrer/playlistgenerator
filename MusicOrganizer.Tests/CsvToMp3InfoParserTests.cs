using MusicOrganizer.Models;
using MusicOrganizer.Services;

namespace MusicOrganizer.Tests;

public class CsvToMp3InfoParserTests
{
    [Theory]
    [InlineData(""","","Can't Buy Me Love","A Tribute To The Beatles","The Welsh National Orchestra",,128000,0,spotify:user:marc.lohrer,2016-10-23T08:15:28Z,"" """, "\"Can't Buy Me Love\"", "\"A Tribute To The Beatles\"", "\"The Welsh National Orchestra\"", "128000", "spotify:user:marc.lohrer", "2016-10-23T08:15:28Z")]
    public static void GetColumns_WhenStringIsInput_ParsedColumnsIsOutput(string input, string trackName, string albumName, string artistNames, string durationMs, string addedBy, string addedAt)
    {
        var columns = CsvToMp3InfoParser.GetColumns(input, CsvToMp3InfoParser.ColumnSeparator);
        var unavailableSpotifySong = UnavailableSpotifySong.FromCsvColumns(columns);

        // Assert
        Assert.Equal(unavailableSpotifySong.TrackName, trackName);
        Assert.Equal(unavailableSpotifySong.AlbumName, albumName);
        Assert.Equal(unavailableSpotifySong.ArtistNames, artistNames);
        Assert.Equal(unavailableSpotifySong.DurationMs, durationMs);
        Assert.Equal(unavailableSpotifySong.AddedBy, addedBy);
        Assert.Equal(unavailableSpotifySong.AddedAt, addedAt);
    }
}

