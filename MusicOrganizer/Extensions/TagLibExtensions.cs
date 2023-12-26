namespace MusicOrganizer;

public static class TagLibExtensions
{
    public static TagLib.File? ParseMp3Tags(this System.IO.FileInfo f)
    {
        try
        {
            var taglibFile = TagLib.File.Create(f.FullName);
            return taglibFile;
        }
        catch (Exception)
        {
            return null;
        }
    }
}

