// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MusicOrganizer.Services;

public interface IFileService
{
    public bool FileExists(FileInfo fileInfo);
    public void MoveFile(FileInfo oldFile, string newFilePath, FileInfo? logger);
}