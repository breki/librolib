using System;

namespace LibroLib.FileSystem
{
    public interface ITemporaryFileStorage
    {
        void Clear();
        string CreateTempDirectory(string prefix);
        void DeleteOldStuff(TimeSpan minimumAge);
        string GetTemporaryFilePath(string fileName);
        string GetTemporaryFilePathWithTimestamp(string fileNameFormat);
        bool IsInTempStorage(string fileName);
    }
}