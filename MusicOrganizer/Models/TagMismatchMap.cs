// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MusicOrganizer.Models;

public class MusicBrainzTagMap
{
    public required string SpotifyTag { get; set; }

    /// <summary>
    /// https://musicbrainz.org/doc/MusicBrainz_Database
    /// </summary>
    public required string MusicBrainzTag { get; set; }
}
