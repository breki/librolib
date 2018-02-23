using LibroLib.Caching;
using LibroLib.FileSystem;
using Moq;
using NUnit.Framework;

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
            fileSystem.Setup(x => x.DoesFileExist(ResourceFullPath))
                .Returns(fileExists);
            Assert.AreEqual(fileExists, cache.IsCached(ResourcePathInCache));
        }

        [Test]
        public void LoadFileFromCache()
        {
            byte[] fileData = new byte[0];

            fileSystem.Setup(x => x.ReadFileAsBytes(ResourceFullPath))
                .Returns(fileData);
            Assert.AreEqual(fileData, cache.Load(ResourcePathInCache));
        }

        [Test]
        public void EnsureCacheSubpathExists()
        {
            fileSystem.Setup(x => x.EnsureDirectoryExists(@"d:\app\Cache\Images"));
            cache.EnsureDirectoryPathExists(ResourcePathInCache, true);
            fileSystem.VerifyAll();
        }

        [Test]
        public void ClearCacheSubpath()
        {
            applicationInfo.Setup(x => x.GetAppDirectoryPath(@"Cache\Images"))
                .Returns(@"d:\app\Cache\Images");
            fileSystem.Setup(x => x.DeleteDirectory(@"d:\app\Cache\Images"));
            cache.ClearCacheDirectory("Images");
            fileSystem.VerifyAll();
        }

        [SetUp]
        public void Setup()
        {
            fileSystem = new Mock<IFileSystem>();
            applicationInfo = new Mock<IApplicationInfo>();
            cache = new DefaultDiskCache(fileSystem.Object, applicationInfo.Object);

            applicationInfo
                .Setup(x => x.GetAppDirectoryPath(@"Cache\Images\image.jpg"))
                .Returns(ResourceFullPath);
        }

        private IDiskCache cache;
        private Mock<IFileSystem> fileSystem;
        private Mock<IApplicationInfo> applicationInfo;
        private const string ResourcePathInCache = @"Images\image.jpg";
        private const string ResourceFullPath = @"d:\app\Cache\Images\image.jpg";
    }
}