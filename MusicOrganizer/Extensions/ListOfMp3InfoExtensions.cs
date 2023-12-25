// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MusicOrganizer.Models;

namespace MusicOrganizer.Extensions
{
    public static class ListOfMp3InfoExtensions
    {
        public static bool IsAmazonIdAnDiscrimator(this List<Mp3Info> mp3Infos)
        {
            var tracksWithAmazonId = mp3Infos.Any(mp3 => !string.IsNullOrEmpty(mp3.AmazonId));
            var tracksWithoutAmazonId = mp3Infos.Any(mp3 => string.IsNullOrEmpty(mp3.AmazonId));
            return tracksWithAmazonId && tracksWithoutAmazonId;
        }
        public record AlbumArtistDiscriminator(bool IsDiscriminator, string Interpret);
        public static AlbumArtistDiscriminator IsAlbumArtistAnDiscrimator(this List<Mp3Info> mp3Infos)
        {
            var tracksWhereInterpretMatchesAlbumArtist = mp3Infos.Any(mp3 => mp3.Interpret.ArrayToString() == mp3.AlbumArtists.ArrayToString());
            var tracksWhereInterpretDontMatchesAlbumArtist = mp3Infos.Any(mp3 => mp3.Interpret.ArrayToString() != mp3.AlbumArtists.ArrayToString());
            var interpretMatchAlbumArtistTracks = mp3Infos.Where(mp3 => mp3.Interpret.ArrayToString() == mp3.AlbumArtists.ArrayToString()).FirstOrDefault();
            return new(tracksWhereInterpretMatchesAlbumArtist
                && tracksWhereInterpretDontMatchesAlbumArtist,
                interpretMatchAlbumArtistTracks?.Interpret.ArrayToString() ?? string.Empty);
        }
    }
}

