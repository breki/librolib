using System.IO;
using LibroLib.Caching;
using LibroLib.FileSystem;
using NUnit.Framework;

namespace LibroLib.Tests.CommonTests
{
    public class DiskCacheIntegrationTests
    {
        [Test]
        public void SaveAndLoadCacheFile()
        {
            const string CacheFileName = @"here\test.dat";
            cache.DeleteCacheFile(CacheFileName);
            cache.Save(CacheFileName, new byte[100]);
            Assert.AreEqual(100, cache.Load(CacheFileName).Length);
        }

        [Test]
        public void DeletingCacheFileShouldReallyDeleteIt()
        {
            const string CacheFileName = @"here\test.dat";
            string fullCacheFileName = applicationInfo.GetAppDirectoryPath (Path.Combine (DefaultDiskCache.DefaultCacheRootDir, CacheFileName));

            cache.Save (CacheFileName, new byte[100]);
            Assert.IsTrue(fileSystem.DoesFileExist(fullCacheFileName));
            cache.DeleteCacheFile (CacheFileName);
            Assert.IsFalse (fileSystem.DoesFileExist(fullCacheFileName));
        }

        [SetUp]
        public void Setup()
        {
            fileSystem = new WindowsFileSystem();
            applicationInfo = new TestApplicationInfo();
            cache = new DefaultDiskCache(fileSystem, applicationInfo);
        }

        private DefaultDiskCache cache;
        private IApplicationInfo applicationInfo;
        private IFileSystem fileSystem;
    }
}