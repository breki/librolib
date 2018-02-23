using System.Diagnostics.Contracts;

namespace LibroLib.Caching
{
    [ContractClass(typeof(IDiskCacheContract))]
    public interface IDiskCache
    {
        void ClearCacheDirectory(string localCacheDirectoryPath);
        void DeleteCacheFile(string localCacheFileName);
        void EnsureDirectoryPathExists(string localCacheFileName, bool isFilePath);
        string GetFullFilePath(string localCachePath);

        /// <summary>
        /// Determines whether the specified file exists in the cache directory structure.
        /// </summary>
        /// <param name="localCachePath">Relative path to the file to be checked.</param>
        /// <returns><c>true</c> if file exists; <c>false</c> otherwise</returns>
        bool IsCached(string localCachePath);

        byte[] Load(string localCachePath);
        void Save(string localCachePath, byte[] data);
    }

    [ContractClassFor(typeof(IDiskCache))]
    // ReSharper disable once InconsistentNaming
    internal abstract class IDiskCacheContract : IDiskCache
    {
        void IDiskCache.ClearCacheDirectory(string localCacheDirectoryPath)
        {
            Contract.Requires(localCacheDirectoryPath != null);
        }

        void IDiskCache.DeleteCacheFile(string localCacheFileName)
        {
            Contract.Requires(localCacheFileName != null);
        }

        void IDiskCache.EnsureDirectoryPathExists(string localCacheFileName, bool isFilePath)
        {
            Contract.Requires(localCacheFileName != null);
        }

        string IDiskCache.GetFullFilePath(string localCachePath)
        {
            Contract.Requires(localCachePath != null);
            Contract.Ensures(Contract.Result<string>() != null);
            throw new System.NotImplementedException();
        }

        bool IDiskCache.IsCached(string localCachePath)
        {
            Contract.Requires(localCachePath != null);
            throw new System.NotImplementedException();
        }

        byte[] IDiskCache.Load(string localCachePath)
        {
            Contract.Requires(localCachePath != null);
            throw new System.NotImplementedException();
        }

        void IDiskCache.Save(string localCachePath, byte[] data)
        {
            Contract.Requires(localCachePath != null);
        }
    }
}