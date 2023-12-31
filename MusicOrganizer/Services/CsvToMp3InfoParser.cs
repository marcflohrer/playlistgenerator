using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MusicOrganizer.Models;

namespace MusicOrganizer.Services;

public static class CsvToMp3InfoParser
{
    public const char ColumnSeparator = ',';
    private const int ColumnCountOfUnvailableSpotifySong = 11;

    public static IList<Mp3Info> ToMp3InfoList(this FileInfo csvFilePlayList, IList<MusicBrainzTagMap> tagMaps)
    {
        ArgumentNullException.ThrowIfNull(csvFilePlayList);
        FixBrokenCsvLines(csvFilePlayList);
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

    private static void FixBrokenCsvLines(FileInfo csvFilePlayList)
    {
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
                    if (IsUnavailableSpotifySong(currentColumnCount))
                    {
                        line = UnavailableSpotifySong
                            .FromCsvColumns(columns)
                            .ToPlaylistEntry()
                            .ToCsvString();
                    }
                    else if (currentColumnCount != headerColumnCount)
                    {
                        throw new InvalidDataException($"CSV format is not consistent: Line {lineCount} in {csvFilePlayList.Name} has {currentColumnCount} columns. Expected is {headerColumnCount} columns.");
                    }
                }
                fixedLines.Add(line);
            }
            csvFilePlayList.WriteLines(fixedLines);
        }
        catch (IOException e)
        {
            Console.WriteLine($"The file could not be read: {e.Message}");
            throw;
        }
        catch (CsvHelperException csvHelperException)
        {
            Console.WriteLine($"Parsing the file failed. {csvHelperException.Message}");
            throw;
        }
    }

    private static bool IsUnavailableSpotifySong(int currentColumnCount) => currentColumnCount == ColumnCountOfUnvailableSpotifySong;

    public static List<string> GetColumns(string line, char columnSeparator)
    {
        var columns = new List<string>();
        var currentColumn = new StringBuilder();
        var inQuotes = false;

        foreach (var currentChar in line)
        {
            if (currentChar == '"')
            {
                // Toggle the state of whether we are inside quotation marks
                inQuotes = !inQuotes;
                // Add quotation marks to the current column
                currentColumn.Append(currentChar);
                continue;
            }

            if (currentChar == columnSeparator && !inQuotes)
            {
                // Column separator outside quotation marks marks the end of a column
                columns.Add(currentColumn.ToString());
                currentColumn.Clear();
            }
            else
            {
                // Add the current character to the current column
                currentColumn.Append(currentChar);
            }
        }

        // Add the last column
        columns.Add(currentColumn.ToString());

        return columns;
    }
}
