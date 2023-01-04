using System;
using Moq;
using MusicOrganizer.Services;

namespace MusicOrganizer.Tests
{
    public class PlaylistServiceTests
    {
        [Theory]
        [InlineData("/Volume/Harddisk/media/",
            "/Volume/Harddisk/media/Florence + The Machine/Lungs/13. You've Got the Love.mp3",
            "Florence + The Machine/Lungs/13. You've Got the Love.mp3")]
        public void GivenPlusInInputStringWhenCreatingPlaylistEntryThenPlusIsInOutputString(string musicDirectory, string mp3File, string expected)
        {
            // Act
            var output = PlaylistService.CreatePlaylistEntry(new DirectoryInfo(musicDirectory), new FileInfo(mp3File), null);

            // Assert
            Assert.Equal(expected, output);
        }
    }
}
