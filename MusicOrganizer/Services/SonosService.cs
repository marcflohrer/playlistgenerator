using MusicOrganizer.Extensions;

namespace MusicOrganizer.Services
{
    public class SonosService
    {
        public SonosService(IFileService fileService)
        {
            FileService = fileService;
        }

        public IFileService FileService { get; }

        public FileInfo ToAsciiOnly(FileInfo fileInfo, FileInfo? logger)
            => fileInfo.RenameToAsciiOnly(FileService, logger);
    }
}

