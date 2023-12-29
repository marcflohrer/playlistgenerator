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

        private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
        {
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            WriteIndented = true
        };
        public static List<FileInfo>? ResumeOrEnumerateMp3s(this Mp3DirectoryInfo mp3Directory, FileInfo logFile, FileInfo resumeFile)
        {
            List<FileInfo>? files;
            if (!resumeFile?.Exists ?? true)
            {
                files = mp3Directory.EnumerateMp3s(logFile);
                StoreResumeFilesPoint(resumeFile, files);
                return files;
            }
            else
            {
                files = LoadResumeFilesPoint(resumeFile!, logFile);
            }
            return files;
        }

        public static List<FileInfo>? LoadResumeFilesPoint(FileInfo resume, FileInfo? output)
        {
            var mp3InfoText = File.ReadAllText(resume.FullName);
            try
            {
                return JsonSerializer.Deserialize<List<string>>(mp3InfoText)?.Select(fn => new FileInfo(fn)).ToList();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(output, ex.Message);
                throw;
            }
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
                    throw;
                }
            }
            throw new InvalidOperationException("Load resume file failed.");
        }

        public static void StoreResumeFilesPoint(FileInfo? resume, List<FileInfo> mp3Files)
        {
            if (resume != null)
            {
                File.WriteAllText(resume.FullName, JsonSerializer.Serialize(mp3Files.Select(mp3 => mp3.FullName).ToList(), s_jsonSerializerOptions));
            }
        }

        public static void StoreResumeTagsPoint(FileInfo? resume, List<FileTags>? fileTags)
        {
            if (resume != null)
            {
                File.WriteAllText(resume.FullName, JsonSerializer.Serialize(fileTags, s_jsonSerializerOptions));
            }
        }

        internal static void Store(Dictionary<string, SongLocations> data, FileInfo resumeFileName)
        {
            if (data != null)
            {
                File.WriteAllText(resumeFileName.FullName, JsonSerializer.Serialize(data, s_jsonSerializerOptions));
            }
        }
    }
}

