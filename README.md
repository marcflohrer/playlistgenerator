# Importify

## Description

Importify is a .NET application designed to import CSV music playlists exported from Spotify using [Exportify](https://exportify.net/). It organizes your local music library and creates m3u playlists based on the provided CSV files. It now includes a feature for mapping music tags between Spotify and MusicBrainz or other MP3 tag databases.

## Features

- Scans and organizes local MP3 files.
- Identifies and logs duplicate MP3 files.
- Deletes empty subdirectories in the music directory.
- Converts Spotify CSV playlists to m3u format.
- Supports operation resumption.
- Maps music tags between Spotify and MusicBrainz or other MP3 tag databases.

## Requirements

- .NET Runtime (compatible with .NET 8.0)
- Local music library in MP3 format
- CSV files exported from Spotify using [Exportify](https://exportify.net/)

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
5. Optionally, configure tag mapping in `appsettings.json`.

## Configuration

In case the song is spelled differently in the spotify playlist and you do not want to fiddle around with your mp3 tags you can add a tag mapping from the spotify tag to way the song is tagged in your local music library. You will find the files that are not found in the file called 'logger.txt' in the folder you passed as parameter '-m'.

- Modify the application's behavior by adjusting the arguments in the `launch.json` file.
- For tag mapping, add your mappings to `appsettings.json` under "TagMismatchMap". Example:

```json
{
  "TagMismatchMap":
  [
    { "SpotifyTag":"Girls Who Play Guitar", "MusicBrainzTag":"Girls Who Play Guitars" },
    { "SpotifyTag":"Wasted Little DJ's", "MusicBrainzTag":"Wasted Little DJs" },
    { "SpotifyTag":"I Gotta Feelin", "MusicBrainzTag":"I Got a Feelin" }
  ]
}
```

## Disclaimer

This software is provided "as is", without warranty of any kind. The author(s) and contributors are not responsible for any damage or loss of data. It is strongly recommended to use Importify only on a backup of your music files.

## License

Importify is licensed under GNU AFFERO GENERAL PUBLIC LICENSE. See the `LICENSE.md` file for more details.

## Acknowledgments

- Thanks to the [Exportify](https://exportify.net/) team for the Spotify playlist export tool.
- Special thanks to all contributors and users of Importify.
