using MusicOrganizer;
using MusicOrganizer.Models;
using PowerArgs;
using System.Text.Json;

internal class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var options = Args.Parse<Options>(args);
            Console.WriteLine($"{DateTime.Now} Options '{JsonSerializer.Serialize(options)}'");
            Application.Run(options);
            Console.WriteLine($"{DateTime.Now} Finished");
        }
        catch (ArgException ex)
        {
            Console.WriteLine($"{DateTime.Now} \n {ex.Message}");
            Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<Options>());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now} \n {ex.Message}");
        }
    }
}
