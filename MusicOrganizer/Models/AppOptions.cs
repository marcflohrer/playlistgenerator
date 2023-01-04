namespace MusicOrganizer.Models
{
    public record AppOptions(List<Mp3DirectoryInfo> MusicDirectories,
        FileInfo LogFile,
        FileInfo ResumeMainFiles,
        FileInfo ResumeMainTags,
        FileInfo ResumePlaylistFiles,
        FileInfo ResumePlaylistTags,
        FileInfo? DeletionScript,
        FileInfo? PlaylistFile);
}

