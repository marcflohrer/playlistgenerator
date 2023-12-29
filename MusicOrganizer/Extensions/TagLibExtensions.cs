namespace MusicOrganizer;

public static class TagLibExtensions
{
    public static TagLib.File? ParseMp3Tags(this FileInfo fileInfo)
    {
        try
        {
            var taglibFile = TagLib.File.Create(fileInfo.FullName);
            return taglibFile;
        }
        catch (Exception)
        {
            throw;
        }
    }
}

