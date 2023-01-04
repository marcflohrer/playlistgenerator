using System;
using MusicOrganizer.Extensions;

namespace MusicOrganizer.Tests;

public class AsciiExtensionsTests
{
    [Theory]
    [InlineData("Not—.mp3", "Not-.mp3")]
    [InlineData("Maxïmo.mp3", "Maximo.mp3")]
    [InlineData("Not¿.mp3", "Not?.mp3")]
    [InlineData("Not½.mp3", "Not0,5.mp3")]
    [InlineData("NotࠀM.mp3", "Not'M.mp3")]
    [InlineData("Notஅ.mp3", "Nota.mp3")]
    [InlineData("ü(1).mp3", "ue(1).mp3")]
    [InlineData("அ(2).mp3", "a(2).mp3")]
    [InlineData("ሊ(3).mp3", "li(3).mp3")]
    [InlineData("℈(4).mp3", "g(4).mp3")]
    [InlineData("Ⰹ(5).mp3", "I(5).mp3")]
    [InlineData("ä.mp3", "ae.mp3")]
    [InlineData("ö.mp3", "oe.mp3")]
    [InlineData("Æ.mp3", "AE.mp3")]
    public static void GivenUnicodeCharInStringWhenCallToToM3uCompliantPathThenOnlyAsciiRemains(string input, string expected)
    {
        var output = input.ToM3uCompliantPath();

        // Assert
        Assert.Equal(expected, output.Text);
    }
}

