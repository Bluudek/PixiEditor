﻿using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using PixiEditor.AvaloniaUI.Helpers;
using PixiEditor.AvaloniaUI.Models.Files;

namespace PixiEditor.AvaloniaUI.Models.IO;

internal class FileTypeDialogDataSet
{
    public enum SetKind { Any, Pixi, Images }
    IEnumerable<FileTypeDialogData> fileTypes;
    string displayName;

    public FileTypeDialogDataSet(SetKind kind, IEnumerable<FileTypeDialogData> fileTypes = null)
    {
        if (fileTypes == null)
            fileTypes = SupportedFilesHelper.GetAllSupportedFileTypes(true);
        var allSupportedExtensions = fileTypes;
        if (kind == SetKind.Any)
        {
            Init("Any", allSupportedExtensions);
        }
        else if (kind == SetKind.Pixi)
        {
            Init("PixiEditor Files", new[] { new FileTypeDialogData(FileType.Pixi) });
        }
        else if (kind == SetKind.Images)
        {
            Init("Image Files", allSupportedExtensions, FileType.Pixi);
        }
    }

    public FileTypeDialogDataSet(string displayName, IEnumerable<FileTypeDialogData> fileTypes, FileType? fileTypeToSkip = null)
    {
        Init(displayName, fileTypes, fileTypeToSkip);
    }

    private void Init(string displayName, IEnumerable<FileTypeDialogData> fileTypes, FileType? fileTypeToSkip = null)
    {
        var copy = fileTypes.ToList();
        if (fileTypeToSkip.HasValue)
            copy.RemoveAll(i => i.FileType == fileTypeToSkip.Value);
        this.fileTypes = copy;

        this.displayName = displayName;
    }

    public FilePickerFileType[] GetFormattedTypes()
    {
        FilePickerFileType[] types = fileTypes.Select(i => i.SaveFilter).ToArray();
        return types;
    }
}
