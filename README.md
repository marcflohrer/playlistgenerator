# Importify

## Description

Importify is a .NET application that imports CSV music playlists exported from Spotify using [Exportify](https://exportify.net/) and creates m3u playlists from them. The application now includes a feature for mapping music tags between Spotify and MusicBrainz or other MP3 tag databases, facilitating better synchronization of song metadata. It also includes a specialized CSV parsing mechanism to handle inconsistencies, specifically for Spotify songs that are currently unavailable and therefore lack some of the fields that are normally present.

## Features

- Scans and organizes local MP3 files.
- Identifies and logs duplicate MP3 files.
- Deletes empty subdirectories in the music directory.
- Converts Spotify CSV playlists to m3u format.
- Supports operation resumption.
- Maps music tags between Spotify and MusicBrainz or other MP3 tag databases.
- Handles and fixes inconsistencies in CSV files, particularly for unavailable Spotify songs.

## Requirements

- .NET Runtime (compatible with .NET 8.0).
- Local music library in MP3 format.
- CSV files exported from Spotify using [Exportify](https://exportify.net/).

## Installation

1. Clone the repository from [[marcflohrer/playlistgenerator]](https://github.com/marcflohrer/playlistgenerator).
2. Build the application using Visual Studio or .NET CLI.

## Usage

1. Place your exported CSV files from Spotify in a designated folder.
2. Ensure your local music library is accessible to the application.
3. Configure the `launch.json` file in VS Code:
   - Update the "program" path to point to the built `MusicOrganizer.dll`.
   - Set the "args" with the paths to your music directory (`-m`) and CSV directory (`-c`).
4. Run the application using VS Code or .NET CLI.
5. Optionally, configure tag mapping in `appsettings.json` for any discrepancies in song metadata.
6. The application will automatically fix any broken CSV files during processing, focusing on unavailable Spotify songs.

## Configuration

To address discrepancies in song metadata between Spotify playlists and your local music library, you can use the tag mapping feature. This is particularly useful if you prefer not to modify your MP3 tags manually. Unmatched files will be listed in 'logger.txt' in the directory specified by the '-m' parameter. Another reason to add a tag mapping is when the artist name contains a character, like in the band name 'Does It Offend You, Yeah?'. Due to parsing rules, everything after the comma is ignored, necessitating a tag mapping that removes the comma.

- Modify the application's behavior by adjusting the arguments in the `launch.json` file.
- For tag mapping, add your mappings to `appsettings.json` under "TagMismatchMap". Example:

```json
{
  "TagMismatchMap":
  [
    { "SpotifyTag":"Girls Who Play Guitar", "MusicBrainzTag":"Girls Who Play Guitars" },
    { "SpotifyTag":"Wasted Little DJ's", "MusicBrainzTag":"Wasted Little DJs" },
    { "SpotifyTag":"I Gotta Feelin", "MusicBrainzTag":"I Got a Feelin" },
    { "SpotifyTag":"Does It Offend You, Yeah?", "MusicBrainzTag":"Does It Offend You Yeah?" }
  ]
}
```

## Disclaimer

This software is provided "as is", without warranty of any kind. The author(s) and contributors are not responsible for any damage or loss of data. Users are strongly recommended to first test Importify on a small subset of their music files and to use it only on a backup of their music library.

## License

Importify is licensed under GNU AFFERO GENERAL PUBLIC LICENSE. See the `LICENSE.md` file for more details.

## Acknowledgments

- Thanks to the [Exportify](https://exportify.net/) team for the Spotify playlist export tool.
- Special thanks to all contributors and users of Importify.
