namespace MusicOrganizer.Models;


public record PlaylistSongs(string NormalizedSongName, List<Mp3Info> Mp3Infos);

public record PlaylistSongInfo(bool IsMainDir, int DurationSeconds, string FileNameNoExt, DirectoryInfo DirectoryInfo);

public record Mp3DirectoryInfo(DirectoryInfo DirectoryInfo, bool isMainDir);

public record SongLocation(string NormalizedSongName, Mp3Info Mp3Info);

public record SongLocations(string NormalizedSongName, List<Mp3Info> Mp3Infos);

public record Mp3Info(string FilePath,
                    string Md5Hash, long SizeInBytes,
                    string[] Interpret,
                    string[] AlbumArtists,
                    string Title,
                    int Number,
                    string AmazonId,
                    int Year,
                    int DurationSeconds);
