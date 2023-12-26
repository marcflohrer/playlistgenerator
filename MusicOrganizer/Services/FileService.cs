namespace MusicOrganizer.Services;

public class FileService : IFileService
{
    public bool FileExists(FileInfo fileInfo) => fileInfo.Exists;

    public void MoveFile(FileInfo oldFile, string destinationPath, FileInfo? logFile)
    {
        if (!Directory.Exists(oldFile.Directory!.FullName))
        {
            return;
        }
        Logger.WriteLine(logFile, $"{DateTime.Now} Moving file {oldFile.FullName} -> {destinationPath}");
        var destinationFile = new FileInfo(destinationPath);
        var dirName = destinationFile.Directory?.FullName;
        if (!Directory.Exists(dirName))
        {
            var _ = dirName != null ? Directory.CreateDirectory(dirName) : null;
        }
        File.Move(oldFile.FullName, destinationPath);
        if (Directory.GetFiles(oldFile.Directory!.FullName).Length == 0 &&
            Directory.GetDirectories(oldFile.Directory!.FullName).Length == 0)
        {
            Directory.Delete(oldFile.Directory!.FullName);
        }
    }
}

