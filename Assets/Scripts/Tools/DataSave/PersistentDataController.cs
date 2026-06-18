using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ILogger = Tools.Logger.ILogger;

namespace Tools.DataSave
{
    public class PersistentDataController
    {
        private readonly string _path;
        private readonly string _fileName;
        private readonly ILogger _logger;
        private readonly int _backups;
        private const string FILES_FOLDER = "data";
        private const string DESCRIPTOR_FILE_NAME = "desciptor.txt";

        private Descriptor _lastSavedDescriptor;
        private FileContentWrapper _fileContentWrapper;

        public PersistentDataController(string path, string fileName, ILogger logger, int backups = 1)
        {
            _path = path;
            _fileName = fileName;
            _logger = logger;
            _backups = backups;
        }

        public bool TryLoad(out string content)
        {
            if (_fileContentWrapper != null)
            {
                content = _fileContentWrapper.Content;
                return true;
            }

            if (TryGetDescriptor(out var descriptor))
            {
                var currentFileId = descriptor.LastSavedId;
                var totalFiles = _backups + 1;

                for (int step = 0; step < totalFiles; step++)
                {
                    var fileId = GetFileId(currentFileId, -step);
                    var filePath = GetFilePath(fileId);

                    if (File.Exists(filePath))
                    {
                        try
                        {
                            var fileJson = File.ReadAllText(filePath);
                            _fileContentWrapper = JsonUtility.FromJson<FileContentWrapper>(fileJson);
                            content = _fileContentWrapper.Content;

                            if (descriptor.LastSavedId != fileId)
                            {
                                var newDescriptor = new Descriptor
                                {
                                    LastSavedId = fileId
                                };

                                if (!TryUpdateDescriptorFile(newDescriptor))
                                {
                                    _logger.LogError("Failed to update descriptor");
                                }

                                _lastSavedDescriptor = newDescriptor;
                            }
                            
                            return true;
                        }
                        catch (Exception e)
                        {
                            _logger.LogException(e);
                            try
                            {
                                File.Delete(filePath);
                            }
                            catch (Exception deleteException)
                            {
                                _logger.LogException(deleteException);
                            }
                            continue;
                        }
                    }
                }
            }

            content = string.Empty;
            return false;
        }

        public bool TrySaveData(string data)
        {
            var fileId = 0;

            if (TryGetDescriptor(out var descriptor))
            {
                //_lastSavedDescriptor = descriptor;
                fileId = GetFileId(descriptor.LastSavedId, + 1);
            }

            var contentWrapper = new FileContentWrapper
            {
                Id = fileId,
                CreationTimeUTC = DateTime.Now.ToFileTimeUtc(),
                Content = data
            };
            
            var filePath = GetFilePath(fileId);

            try
            {
                var newDescriptor = new Descriptor
                {
                    LastSavedId = fileId
                };
                WriteTextAndCreateDirectory(filePath, JsonUtility.ToJson(contentWrapper));
                TryUpdateDescriptorFile(newDescriptor);
                //add handling of failed descriptor save
                //descriptor should be refferenced on prev file
                //_lastSavedDescriptor = newDescriptor;
                _fileContentWrapper = contentWrapper;
                return true;
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return false;
            }
        }

        public void ClearAllData()
        {
            ClearDirectoryContent();
        }

        private void WriteTextAndCreateDirectory(string path, string text)
        {
            var directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(path, text);
        }

        private string GetFilePath(int id)
        {
            return Path.Combine(_path, FILES_FOLDER, _fileName, $"{_fileName}_{id}");
        }

        private string GetDescriptorPath()
        {
            return Path.Combine(_path, FILES_FOLDER, _fileName, DESCRIPTOR_FILE_NAME);
        }

        private int GetFileId(int currentFileId, int step)
        {
            var filesAmount = _backups + 1;

            if (step < 0)
            {
                step = filesAmount + step % filesAmount;
            }

            return (currentFileId + step) % (filesAmount);
        }

        private bool TryGetDescriptor(out Descriptor descriptor)
        {
            if (_lastSavedDescriptor == null)
            {
                var descriptorPath = GetDescriptorPath();

                if (File.Exists(descriptorPath))
                {
                    try
                    {
                        var descriptorJson = File.ReadAllText(descriptorPath);
                        _lastSavedDescriptor = JsonUtility.FromJson<Descriptor>(descriptorJson);
                        descriptor = _lastSavedDescriptor;
                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.Log(
                            $"Failed to read descriptor of file {_fileName}, at path {descriptorPath}. Trying to create new one. Ex: {e.Message}, Trace: {e.StackTrace}");
                    }
                }
                
                if (!TryCreateDescriptorForExistingFiles(out var createdDescriptor))
                {
                    ClearDirectoryContent();
                    descriptor = default;
                    return false;
                }

                _lastSavedDescriptor = createdDescriptor;
                descriptor = createdDescriptor;
                return true;
            }

            descriptor = _lastSavedDescriptor;
            return true;
        }

        private void ClearDirectoryContent()
        {
            var directoryPath = Path.GetDirectoryName(GetDescriptorPath());

            if (Directory.Exists(directoryPath))
            {
                var di = new DirectoryInfo(directoryPath);
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete(); 
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    dir.Delete(true); 
                }
            }
        }

        private bool TryCreateDescriptorForExistingFiles(out Descriptor descriptor)
        {
            List<FileContentWrapper> wrappers = new();
            var totalFiles = _backups + 1;

            for (int step = 0; step < totalFiles; step++)
            {
                var fileId = GetFileId(0, step);
                var filePath = GetFilePath(fileId);

                if (File.Exists(filePath))
                {
                    try
                    {
                        var fileJson = File.ReadAllText(filePath);
                        var contentWrapper = JsonUtility.FromJson<FileContentWrapper>(fileJson);

                        wrappers.Add(contentWrapper);
                    }
                    catch (Exception e)
                    {
                        _logger.LogException(e);
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch (Exception exception)
                        {
                            _logger.LogException(exception);
                        }
                        
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }

            if (wrappers.Count == 0)
            {
                descriptor = default;
                return false;
            }

            var actualWrapper = wrappers[0];
            for (var index = 1; index < wrappers.Count; index++)
            {
                if (actualWrapper.CreationTimeUTC < wrappers[index].CreationTimeUTC)
                {
                    actualWrapper = wrappers[index];
                }
            }

            descriptor = new Descriptor
            {
                LastSavedId = actualWrapper.Id
            };

            if (!TryUpdateDescriptorFile(descriptor))
            {
                return false;
            }

            return true;
        }

        private bool TryUpdateDescriptorFile(Descriptor descriptor)
        {
            var descriptorPath = GetDescriptorPath();
            var directoryPath = Path.GetDirectoryName(descriptorPath);
            if (Directory.Exists(directoryPath))
            {
                try
                {
                    WriteTextAndCreateDirectory(descriptorPath, JsonUtility.ToJson(descriptor, true));
                }
                catch (Exception e)
                {
                    _logger.LogException(e);
                    return false;
                }
            }

            return true;
        }


        [Serializable]
        private class FileContentWrapper
        {
            [SerializeField]
            public long CreationTimeUTC;
            [SerializeField]
            public int Id;
            [SerializeField]
            public string Content;
        }

        [Serializable]
        private class Descriptor
        {
            [SerializeField]
            public int LastSavedId;
        }
    }
}