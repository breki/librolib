using System.Diagnostics.Contracts;
using System.IO;
using LibroLib.FileSystem;

namespace LibroLib.Caching
{
    public class DefaultDiskCache : IDiskCache
    {
        public DefaultDiskCache (
            IFileSystem fileSystem,
            IApplicationInfo applicationInfo)
        {
            Contract.Requires(fileSystem != null);
            Contract.Requires(applicationInfo != null);
            this.fileSystem = fileSystem;
            this.applicationInfo = applicationInfo;
        }

        public DefaultDiskCache(
            IFileSystem fileSystem,
            string cacheRootDirectory)
        {
            this.fileSystem = fileSystem;
            this.cacheRootDirectory = cacheRootDirectory;
        }

        public string GetFullFilePath(string localCachePath)
        {
            string cachePath = Path.Combine(cacheRootDirectory, localCachePath);
            if (applicationInfo != null)
                return applicationInfo.GetAppDirectoryPath(cachePath);
            return Path.GetFullPath(cachePath);
        }

        public void ClearCacheDirectory(string localCacheDirectoryPath)
        {
            string fullPath = GetFullFilePath(localCacheDirectoryPath);
            fileSystem.DeleteDirectory(fullPath);
        }

        public void DeleteCacheFile(string localCacheFileName)
        {
            fileSystem.DeleteFile(Path.Combine(cacheRootDirectory, localCacheFileName), false);
        }

        public void EnsureDirectoryPathExists(string localCacheFileName, bool isFilePath)
        {
            string fullPath = GetFullFilePath(localCacheFileName);
            if (isFilePath)
                fileSystem.EnsureDirectoryExists(Path.GetDirectoryName(fullPath));
            else
                fileSystem.EnsureDirectoryExists(fullPath);
        }

        public bool IsCached(string localCachePath)
        {
            return fileSystem.DoesFileExist(GetFullFilePath(localCachePath));
        }

        public byte[] Load(string localCachePath)
        {
            return fileSystem.ReadFileAsBytes(GetFullFilePath(localCachePath));
        }

        public void Save(string localCachePath, byte[] data)
        {
            EnsureDirectoryPathExists(localCachePath, true);
            string fullPath = GetFullFilePath(localCachePath);
            fileSystem.WriteFile(fullPath, data);
        }

        public const string DefaultCacheRootDir = "Cache";

        private readonly string cacheRootDirectory = DefaultCacheRootDir;
        private readonly IApplicationInfo applicationInfo;
        private readonly IFileSystem fileSystem;
    }
}
