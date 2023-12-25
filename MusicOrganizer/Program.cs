// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            var optionsString = JsonSerializer.Serialize(options);
            Console.WriteLine(optionsString);
            //Console.WriteLine($"{DateTime.Now} Options '{optionsString}'");
            Application.Run(options);
            Console.WriteLine($"Finished");
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
