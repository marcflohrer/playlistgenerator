using System;
using System.Text.RegularExpressions;
using MusicOrganizer.Extensions;
using Moq;
using static MusicOrganizer.Extensions.SonosEncodingExtensions;
using MusicOrganizer.Services;

namespace MusicOrganizer.Tests
{
    public class SonosServiceTests
    {
        [Theory]
        [InlineData("03. It’s Not—My Time (Live in Austria).mp3", "03. It's Not-My Time (Live in Austria)(1).mp3")]
        [InlineData("Maxïmo(1).mp3", "Maximo(2).mp3")]
        [InlineData("03. It‵s Not¿My Time (Live in Austria)(a).mp3", "03. It's Not?My Time (Live in Austria)(a)(1).mp3")]
        [InlineData("03. It‘s Not½My Time (Live in Austria)().mp3", "03. It's Not0,5My Time (Live in Austria)()(1).mp3")]
        [InlineData("03. It’s NotࠀMy Time (Live in Austria)(000).mp3", "03. It's Not'My Time (Live in Austria)(1).mp3")]
        [InlineData("03. It‛s NotஅMyሊTime℈(LiveⰉin Austria)(2).mp3", "03. It's NotaMyliTimeg(LiveIin Austria)(3).mp3")]
        [InlineData("ü(1).mp3", "ue(2).mp3")]
        [InlineData("அ(2).mp3", "a(3).mp3")]
        [InlineData("ሊ(3).mp3", "li(4).mp3")]
        [InlineData("℈(4).mp3", "g(4).mp3")]
        [InlineData("Ⰹ(5).mp3", "I(6).mp3")]
        [InlineData("03. Träume sterben jung(2).mp3", "03. Traeume sterben jung(3).mp3")]
        [InlineData("03. Völkerball(2).mp3", "03. Voelkerball(3).mp3")]
        [InlineData("03. Ænema.mp3", "03. AEnema(1).mp3")]
        public void Test1(string input, string expected)
        {
            // Arrange
            var mp3File = new FileInfo(input);
            var fileService = new Mock<IFileService>();

            fileService.Setup(m => m.FileExists(It.Is<FileInfo>(f => !f.Name.EndsWith(expected)))).Returns(true);

            // Act
            var sonosService = new SonosService(fileService.Object);
            mp3File = sonosService.ToAsciiOnly(mp3File, new FileInfo("logger"));

            // Assert
            Assert.Equal(expected, mp3File.Name);
        }

        [Theory]
        [InlineData("03. It’s Not—My Time (Live in Austria).mp3", "03. It’s Not—My Time (Live in Austria)(1).mp3")]
        [InlineData("03. It′s NotïMy Time (Live in Austria)(1).mp3", "03. It′s NotïMy Time (Live in Austria)(2).mp3")]
        [InlineData("03. It‵s Not¿My Time (Live in Austria)(a).mp3", "03. It‵s Not¿My Time (Live in Austria)(a)(1).mp3")]
        [InlineData("03. It‘s Not½My Time (Live in Austria)().mp3", "03. It‘s Not½My Time (Live in Austria)()(1).mp3")]
        [InlineData("03. It’s NotࠀMy Time (Live in Austria)(000).mp3", "03. It’s NotࠀMy Time (Live in Austria)(1).mp3")]
        [InlineData("03. It’s NotࠀMy Time (Live in Austria)(010).mp3", "03. It’s NotࠀMy Time (Live in Austria)(11).mp3")]
        [InlineData("03. It‛s NotஅMyሊTime℈(LiveⰉin Austria)(2).mp3", "03. It‛s NotஅMyሊTime℈(LiveⰉin Austria)(3).mp3")]
        public void GivenIndexIs1_WhenIncrementing_IndexIs2(string input, string expected)
        {
            // Act
            var output = SonosEncodingExtensions.IncrementDuplicateCounter(input);

            // Assert
            Assert.Equal(expected, output);
        }
    }
}

