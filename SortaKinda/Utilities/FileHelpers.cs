using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;

namespace SortaKinda.Utilities;

public static class FileHelpers {
    private static readonly Dictionary<string, Task> FileSavingTasks = [];

    private static readonly JsonSerializerOptions SerializerOptions = new() {
        WriteIndented = true,
        IncludeFields = true,
    };

    public static T LoadFile<T>(string filePath, T? defaultObject = null) where T : class, new() {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo is { Exists: true }) {
            try {
                var fileText = IReliableFileStorage.Get().ReadAllTextAsync(fileInfo.FullName).Result;
                var dataObject = JsonSerializer.Deserialize<T>(fileText, SerializerOptions);

                // If deserialize result is null, create a new instance instead and save it.
                if (dataObject is null) {
                    dataObject = defaultObject ?? new T();
                    SaveFile(dataObject, filePath);
                }

                return dataObject;
            }
            catch (Exception e) {
                // If there is any kind of error loading the file, generate a new one instead and save it.
                IPluginLog.Get().Error(e, $"Error trying to load file {filePath}, creating a new one instead.");

                SaveFile(defaultObject ?? new T(), filePath);
            }
        }

        var newFile = defaultObject ?? new T();
        SaveFile(newFile, filePath);

        return newFile;
    }

    public static void SaveFile<T>(T? file, string filePath) {
        try {
            if (file is null) {
                IPluginLog.Get().Error("Null file provided.");
                return;
            }

            var fileText = JsonSerializer.Serialize(file, file.GetType(), SerializerOptions);

            if (FileSavingTasks.TryGetValue(filePath, out var task)) {
                if (task.IsCompleted) {
                    FileSavingTasks[filePath] = IReliableFileStorage.Get().WriteAllTextAsync(filePath, fileText);
                }
                else if (task.IsFaulted) {
                    throw task.Exception;
                }
                else if (task.Status is TaskStatus.Running) {
                    IPluginLog.Get().Debug($"File save for {filePath} in progress, trying again.");
                    IFramework.Get().RunOnTick(() => {
                        SaveFile(file, filePath); // try again
                    });
                }
            }
            else {
                FileSavingTasks[filePath] = IReliableFileStorage.Get().WriteAllTextAsync(filePath, fileText);
            }
        }
        catch (Exception e) {
            IPluginLog.Get().Error(e, $"Error trying to save file {filePath}");
        }
    }

    public static byte[] LoadBinaryFile(int length, string filePath) {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo is { Exists: true }) {
            try {
                var dataObject = IReliableFileStorage.Get().ReadAllBytesAsync(fileInfo.FullName).Result;

                // If deserialize result is null, create a new instance instead and save it.
                if (dataObject.Length != length) {
                    dataObject = new byte[length];
                    SaveFile(dataObject, filePath);
                }

                return dataObject;
            }
            catch (Exception e) {
                // If there is any kind of error loading the file, generate a new one instead and save it.
                IPluginLog.Get().Error(e, $"Error trying to load file {filePath}, creating a new one instead.");

                SaveFile(new byte[length], filePath);
            }
        }

        var newFile = new byte[length];
        SaveFile(newFile, filePath);

        return newFile;
    }

    public static void SaveBinaryFile(byte[] data, string filePath) {
        try {
            if (FileSavingTasks.TryGetValue(filePath, out var task)) {
                if (task.IsCompleted) {
                    FileSavingTasks[filePath] = IReliableFileStorage.Get().WriteAllBytesAsync(filePath, data);
                }
                else if (task.IsFaulted) {
                    throw task.Exception;
                }
                else if (task.Status is TaskStatus.Running) {
                    IPluginLog.Get().Debug($"File save for {filePath} in progress, trying again.");
                    IFramework.Get().RunOnTick(() => {
                        SaveBinaryFile(data, filePath); // try again
                    });
                }
            }
            else {
                FileSavingTasks[filePath] = IReliableFileStorage.Get().WriteAllBytesAsync(filePath, data);
            }
        }
        catch (Exception e) {
            IPluginLog.Get().Error(e, $"Error trying to save binary data {filePath}");
        }
    }

    public static FileInfo GetFileInfo(params string[] path) {
        var directory = SortaKinda.PluginInterface.ConfigDirectory;

        for (var index = 0; index < path.Length - 1; index++) {
            directory = new DirectoryInfo(Path.Combine(directory.FullName, path[index]));
            if (!directory.Exists) {
                directory.Create();
            }
        }

        return new FileInfo(Path.Combine(directory.FullName, path[^1]));
    }

    public static string GetCharacterPath() {
        if (!IClientState.Get().IsLoggedIn) {
            throw new Exception("Character is not logged in.");
        }

        return IPlayerState.Get().ContentId.ToString("X");
    }
}
