<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Thanks again! Now go create something AMAZING! :D
-->
<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->

<br />

  <h3 align="center">Playlist Generator</h3>

  <p align="center">
    <br />
    <a href="https://github.com/marcflohrer/playlistgenerator"><strong>Explore the docs »</strong></a>
  </p>
</p>

<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li><a href="#Prerequisites">What is it about?</a></li>
    <li><a href="#known-limitations">Known limitations</a></li>
    <li><a href="#get-started">Get started</a></li>
    <li><a href="#what-it-does">What it does</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

This project provides a M3U playlist generator. The special focus was that the M3U playlist generated are compatible with the SONOS system.

### Prerequisites

You need docker on the machine where you want to run the application:

* An IDE to build and maybe also to run the project, e.g. Visual Studio will do
* [dotnet 7.0](https://dotnet.microsoft.com/download/dotnet/7.0)
* Any hardware that runs dotnet 7.0

### Known Limitations

* Only MP3 files are supported.
* Only M3U playlists will be created.
* Deletion script created only works on bash terminals.

### Get started

1) Build the project with the IDE of your choice.
2) Create a folder and copy the MP3 files that should be on your playlist into this folder
3) Pass the following parameters to the executable your created in step 1.

```bash
mo -p <path-to-your-mp3-music-library> -l <path-to-your-mp3-playlist-folder> -n PlaylistName.m3u -f <filename-used-as-restore-point-for-mp3-files> -t <filename-used-as-restore-point-for-mp3-tags> -o logfile.txt -d deletionScript.sh
```

A valid parameter set on Mac machine would be this:

```bash
mo -p /Volume/Harddisk/music -l /Volume/Harddisk/playlist -n PlaylistName.m3u -f restorefiles.txt -t restoretags.txt -o logfile.txt -d deletionScript.sh
```

### What it does

1) After scanning the files it will create a restore point in the filename you provided with parameter ```-f```. This speeds up future runs of the application. One file is stored in the folder of the mp3 music library and one is stored in the playlist folder. In case the files have changed this restore file should be deleted or manually updated to keep it in sync with reality.
2) After scanning the tags it will create a restore point in the filename you provided with parameter ```-t```. This speeds up future runs of the application. One file is stored in the folder of the mp3 music library and one is stored in the playlist folder. In case the files have changed this restore file should be deleted or manually updated to keep it in sync with reality.
3) After scanning the tags it checks your input and playlist folder for duplicates and aborts if it finds duplicates. In this case it outputs a deletion script with the name you provided as paramter ```-d```. The deletion script when executed will delete the duplicate MP3 files. This decision is based upon the tags in the MP3 files and might be erroneous. So check the script carefully before executing it - maybe the tags are wrong.
3.1) **How does it decide what file to keep?** If there are two or more songs and only one of the has an Amazon ID it will put the other two files in the deletion script. If all files or no file has an Amazon ID it will check if in of the files the album artist differs from the song artist. If so it will put the files where the album artists differs onto the deletion list. If there are still duplicates it will keep the larger file over the shorter file. In case none of these criteria help to narrow down the duplicates to one remaining file it will take one random duplicate.
4) After scanning the folders for duplicates it creates a playlist in M3U format in the folder of the mp3 music library with the name you provided with parameter ```-n```.

## License

This project is licensed under the Reciprocal Public License 1.5 (RPL1.5). This is a GPL-style license with very detailed liability protection and numerous requirements, created to close a perceived loophole in the GPL which let users sell modified software without 'fairly' distributing it. Disputes over this license must be settled through an American arbitration process.
