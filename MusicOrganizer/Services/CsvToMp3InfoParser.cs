using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MusicOrganizer.Models;
namespace MusicOrganizer.Services;

public static class CsvToMp3InfoParser
{
    private const char ColumnSeparator = ',';

    public static IList<Mp3Info> ToMp3InfoList(this FileInfo csvFilePlayList, IList<MusicBrainzTagMap> tagMaps)
    {
        ArgumentNullException.ThrowIfNull(csvFilePlayList);
        FixBrokenCsv(csvFilePlayList);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = Environment.NewLine
        };
        using var reader = new StreamReader(path: csvFilePlayList.FullName);
        using var csv = new CsvReader(reader, config, false);
        var exportifyPlaylist = csv.GetRecords<ExportifyPlaylistEntry>();
        var playlistEntries = new List<Mp3Info>();
        foreach (var playlistEntry in exportifyPlaylist)
        {
            playlistEntries.Add(playlistEntry.ToPlaylistEntry(tagMaps));
        }
        return playlistEntries;
    }

    private static void FixBrokenCsv(FileInfo csvFilePlayList)
    {
        using var reader = new StreamReader(path: csvFilePlayList.FullName);
        try
        {
            // Open the text file using a stream reader.
            using var streamReader = new StreamReader(csvFilePlayList.FullName);
            // Read the stream as a string, and write the string to the console.
            var fixedLines = new List<string>();
            string? line;
            var isHeader = true;
            var headerColumnCount = -1;
            var lineCount = 0;
            while ((line = streamReader.ReadLine()) != null)
            {
                ++lineCount;
                if (isHeader)
                {
                    var columns = GetColumns(line, ColumnSeparator);
                    headerColumnCount = columns.Count;
                    isHeader = false;
                }
                else
                {
                    var columns = GetColumns(line, ColumnSeparator);
                    var currentColumnCount = columns.Count;
                    if (IsUnavailableSpotifySong(headerColumnCount, currentColumnCount))
                    {
                        columns = GetColumns(line, ColumnSeparator);
                        line = UnavailableSpotifySong
                            .FromCsvColumns(columns)
                            .ToPlaylistEntry()
                            .ToCsvString();
                    }
                    else if (currentColumnCount != headerColumnCount)
                    {
                        throw new InvalidDataException($"CSV format is not consistent: Line {lineCount} in {csvFilePlayList.Name} has {currentColumnCount} Columns. Expected is {headerColumnCount}");
                    }
                }
                fixedLines.Add(line);
            }
            WriteFixedLines(fixedLines, csvFilePlayList);
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }

    private static bool IsUnavailableSpotifySong(int headerColumnCount, int currentColumnCount)
    {
        return currentColumnCount == headerColumnCount - 12;
    }

    private static List<string> GetColumns(string line, char columnSeparator)
    {
        var columns = new List<string>();
        var currentColumnCount = 0;
        var inBetweenBrackets = false;
        char? currentChar = null;
        var currentColumn = new StringBuilder();
        foreach (var ch in line)
        {
            currentChar = ch;
            if (currentChar != columnSeparator)
            {
                currentColumn.Append(currentChar);
            }
            // Ignore column separators in between quotes
            if (inBetweenBrackets == false && currentChar == '"')
            {
                inBetweenBrackets = true;
                continue;
            }
            if (inBetweenBrackets == true)
            {
                if (currentChar == '"')
                {
                    inBetweenBrackets = false;
                }
                continue;
            }
            if (currentChar == columnSeparator)
            {
                columns.Add(currentColumn.ToString());
                currentColumn = new StringBuilder();
                ++currentColumnCount;
            }
        }
        if (currentChar != columnSeparator)
        {
            columns.Add(currentColumn.ToString());
            currentColumn = new StringBuilder();
            ++currentColumnCount;
        }

        return columns;
    }

    private static void WriteFixedLines(List<string> fixedLines, FileInfo csvFilePlayList)
    {
        using var streamWriter = new StreamWriter(csvFilePlayList.FullName, false, Encoding.UTF8);
        foreach (var line in fixedLines)
        {
            streamWriter.WriteLine(line);
        }
    }
}
