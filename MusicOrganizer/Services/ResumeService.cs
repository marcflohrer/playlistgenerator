// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using System.Text.Json.Serialization;
using MusicOrganizer.Models;
using static MusicOrganizer.Services.Mp3TagService;

namespace MusicOrganizer.Services
{
    public static class ResumeService
    {

        public static List<FileInfo> ResumeOrEnumerateMp3s(this List<Mp3DirectoryInfo> mp3Directories, FileInfo logFile, FileInfo resumeFile)
        {
            var files = LoadResumeFilesPoint(resumeFile, logFile);
            if (files == null)
            {
                files = mp3Directories.EnumerateMp3s(logFile);
                StoreResumeFilesPoint(resumeFile, files);
            }

            return files;
        }

        public static List<FileInfo>? LoadResumeFilesPoint(FileInfo? resume, FileInfo? output)
        {
            if (resume != null && resume.Exists)
            {
                var mp3InfoText = File.ReadAllText(resume.FullName);
                try
                {
                    return JsonSerializer.Deserialize<List<string>>(mp3InfoText)?.Select(fn => new FileInfo(fn)).ToList();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(output, ex.Message);
                    return null;
                }
            }
            return null;
        }

        public static List<FileTags>? LoadResumeTagsPoint(FileInfo? resume)
        {
            if (resume != null && resume.Exists)
            {
                var mp3InfoText = File.ReadAllText(resume.FullName);
                try
                {
                    return JsonSerializer.Deserialize<List<FileTags>>(mp3InfoText);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public static void StoreResumeFilesPoint(FileInfo? resume, List<FileInfo> mp3Files)
        {
            if (resume != null)
            {
                File.WriteAllText(resume.FullName, JsonSerializer.Serialize(mp3Files.Select(mp3 => mp3.FullName).ToList(), new JsonSerializerOptions
                {
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                    WriteIndented = true
                }));
            }
        }

        public static void StoreResumeTagsPoint(FileInfo? resume, List<FileTags> fileTags)
        {
            if (resume != null)
            {
                File.WriteAllText(resume.FullName, JsonSerializer.Serialize(fileTags, new JsonSerializerOptions
                {
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                    WriteIndented = true
                }));
            }
        }

        internal static void Store(Dictionary<string, SongLocations> data, FileInfo resumeFileName, FileInfo? output)
        {
            if (data != null)
            {
                File.WriteAllText(resumeFileName.FullName, JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                    WriteIndented = true
                }));
            }
        }
    }
}

