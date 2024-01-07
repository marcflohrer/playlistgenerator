using MusicOrganizer.Extensions;
using MusicOrganizer.Models;
using MusicOrganizer.Tests.TestDataGenerator;
using static MusicOrganizer.Extensions.FindDuplicatesExtensions;

namespace MusicOrganizer.Tests;

public class FindDuplicatesExtensionsTests
{
    [Theory]
    [MemberData(nameof(RemoveDuplicatesTests.RemoveDuplicatesWithDifferentAmazonIdTestDataGenerator.DuplicateSongTestInput),
        MemberType = typeof(RemoveDuplicatesTests.RemoveDuplicatesWithDifferentAmazonIdTestDataGenerator))]
    public static void RemoveDuplicatesWithDifferentAmazonId_ShouldRemoveDuplicatesWithDifferentAmazonId(ICollection<string> amazonIds, int expectedDuplicatesCount, int expectedMp3Infos)
    {
        // Given
        var normalizedTitle = "normalizedTitle";
        var normalizedTitleInternal = "normalizedTitleInternal";
        var input = new Dictionary<string, SongLocations>
        {
            { 
                normalizedTitle, new SongLocations(normalizedTitleInternal, amazonIds.Select(a => DefaultMp3Info(a)).ToList()) 
            }
        };

        // When
        var output = DuplicatesWithDifferentAmazonIdsAreNotDuplicates(input);

        // Then
        Assert.Equal(expectedDuplicatesCount, output.Count);
        if(expectedDuplicatesCount > 0)
        {
            Assert.Equal(expectedMp3Infos, output[normalizedTitle].Mp3Infos.Count);
            Assert.Equal(normalizedTitleInternal, output[normalizedTitle].NormalizedTitle);
        }
    }

    private static Mp3Info DefaultMp3Info(string amazonId) => new (
        "FilePath1", 50000, "MainArtist1",
                    ["Interpret1"] ,
                    ["AlbumArtists1"],
                    "Title1",
                    1,
                    amazonId,
                    2024,
                    300
    );
}

