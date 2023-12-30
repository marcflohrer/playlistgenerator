using MusicOrganizer.Extensions;
using MusicOrganizer.Models;
using MusicOrganizer.Services;

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

    [Theory]
    [InlineData("FooBar  - Radio Edit", "FooBar")]
    [InlineData("FooBar Radio Edit", "FooBar Radio Edit")]
    [InlineData("Miss Murray - Radio Mix", "Miss Murray")]
    public static void RemoveContentBeforeDash_WhenDash_ThenOnlyTextBeforeDashIsReturned(string input, string expected)
    {
        var output = input.RemoveContentAfterDash(Models.NormalizeMode.Strict);

        // Assert
        Assert.Equal(expected, output);
    }    

    [Theory]
    [InlineData("Rock \u0026 Roll Queen 2020 (German Version)", false, "rockrollqueen2020")]
    [InlineData("An Honest Mistake - CD Album Version", false, "anhonestmistake")]
    [InlineData("Jennifer Rostock feat. Feine Sahne Fischfilet", true, "jenniferrostock")]
    [InlineData("Hard-FI", true, "hard-fi")]
    [InlineData("Hard‐Fi", true, "hard-fi")]
    [InlineData("Martin Solveig & Dragonettes", true, "martinsolveig")]
    [InlineData("I Love It (feat. Charli XCX)", false, "iloveit")]
    [InlineData("Many Shades Of Black - Performed by The Raconteurs and Adele", true, "manyshadesofblack")]
    [InlineData("We Are Your Friends - Justice Vs Simian", false, "weareyourfriends")]
    [InlineData("Wasted Little DJ's", false, "wastedlittledjs")]
    [InlineData("Down & Out I", false, "downouti")]
    [InlineData("Down & Out II", false, "downoutii")]
    [InlineData("Justice Vs. Simian", true, "justice")]
    [InlineData("Justice vs. Simian", true, "justice")]
    [InlineData("Justice Vs Simian", true, "justice")]
    [InlineData("Justice vs Simian", true, "justice")]
    [InlineData("I Follow Rivers (Live @ Giel! - VARA/3FM)", true, "ifollowrivers")]
    public static void NormalizeSongTitle_WhenBrackets_ThenOnlyTextBeforeBracketsIsReturned(string input, bool isInterpret, string expected)
    {
        var normalizeMode = isInterpret ? NormalizeMode.StrictInterpret : NormalizeMode.Strict;
        var output = input.NormalizeSongTag(new List<MusicBrainzTagMap>{
            new() {
                SpotifyTag = "Wasted Little DJ's",
                MusicBrainzTag = "Wasted Little DJs"
            }
        }, normalizeMode);

        // Assert
        Assert.Equal(expected, output);
    }
}

