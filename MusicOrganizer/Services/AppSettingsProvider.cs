using Microsoft.Extensions.Configuration;
using MusicOrganizer.Models;

namespace MusicOrganizer.Services;

public class AppSettingsProvider
{
    public static IList<MusicBrainzTagMap> GetTagMaps()
    {
        // Konfigurationsbuilder erstellen
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        // Konfiguration erstellen
        var configuration = builder.Build();
        // Oder für komplexere Konfigurationen
        var tagMismatches = new List<MusicBrainzTagMap>();
        configuration.GetSection("TagMismatchMap").Bind(tagMismatches);
        return tagMismatches;
    }
}
