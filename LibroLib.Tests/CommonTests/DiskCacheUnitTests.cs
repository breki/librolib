using LibroLib.Caching;
using LibroLib.FileSystem;
using NUnit.Framework;
using Rhino.Mocks;

namespace LibroLib.Tests.CommonTests
{
    public class DiskCacheUnitTests
    {
        [Test]
        public void CheckDiskCachePaths()
        {
            Assert.AreEqual(
                ResourceFullPath, 
                cache.GetFullFilePath(ResourcePathInCache));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IfFileIsOnDiskItCanBeRegardedAsCached(bool fileExists)
        {
            fileSystem.Expect(x => x.DoesFileExist(ResourceFullPath)).Return(fileExists);
            Assert.AreEqual(fileExists, cache.IsCached(ResourcePathInCache));
        }

        [Test]
        public void LoadFileFromCache()
        {
            byte[] fileData = new byte[0];

            fileSystem.Expect(x => x.ReadFileAsBytes(ResourceFullPath)).Return(fileData);
            Assert.AreEqual(fileData, cache.Load(ResourcePathInCache));
        }

        [Test]
        public void EnsureCacheSubpathExists()
        {
            fileSystem.Expect(x => x.EnsureDirectoryExists(@"d:\app\Cache\Images"));
            cache.EnsureDirectoryPathExists(ResourcePathInCache, true);
            fileSystem.VerifyAllExpectations();
        }

        [Test]
        public void ClearCacheSubpath()
        {
            applicationInfo.Expect(x => x.GetAppDirectoryPath(@"Cache\Images"))
                .Return(@"d:\app\Cache\Images");
            fileSystem.Expect(x => x.DeleteDirectory(@"d:\app\Cache\Images"));
            cache.ClearCacheDirectory("Images");
            fileSystem.VerifyAllExpectations();
        }

        [SetUp]
        public void Setup()
        {
            fileSystem = MockRepository.GenerateMock<IFileSystem>();
            applicationInfo = MockRepository.GenerateStub<IApplicationInfo>();
            cache = new DefaultDiskCache(fileSystem, applicationInfo);

            applicationInfo.Expect(x => x.GetAppDirectoryPath(@"Cache\Images\image.jpg"))
                .Return(ResourceFullPath);
        }

        private IDiskCache cache;
        private IFileSystem fileSystem;
        private IApplicationInfo applicationInfo;
        private const string ResourcePathInCache = @"Images\image.jpg";
        private const string ResourceFullPath = @"d:\app\Cache\Images\image.jpg";
    }
}