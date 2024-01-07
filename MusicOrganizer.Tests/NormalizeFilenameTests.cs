using MusicOrganizer.Extensions;

namespace MusicOrganizer.Tests;

public class NormalizeFilenameTests
{
    [Theory]
    [InlineData("03. It’s Not My Time (Live in Austria).mp3", "03. It’s Not My Time .mp3")]
    [InlineData("03. It’s Not My Time [Live in Austria].mp3", "03. It’s Not My Time .mp3")]
    [InlineData("03. It’s Not My Time {Live in Austria}.mp3", "03. It’s Not My Time .mp3")]
    public void RemoveContentInBrackets_ShouldRemoveContentInBrackets(string input, string expected)
    {
        string filename = input;

        var normalized = filename.RemoveContentInBrackets();

        Assert.Equal(expected, normalized);
    }
}
