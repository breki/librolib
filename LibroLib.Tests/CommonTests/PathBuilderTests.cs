using System;
using LibroLib.FileSystem;
using NUnit.Framework;

namespace LibroLib.Tests.CommonTests
{
    public class PathBuilderTests
    {
        [Test]
        public void EmptyPath()
        {
            Assert.Throws<ArgumentException>(
                () => new PathBuilder(string.Empty));
        }

        [Test]
        public void ReadRelativePath()
        {
            PathBuilder b = new PathBuilder(@"dir1\dir2\file.txt");
            Assert.AreEqual(3, b.PathDepth);
            Assert.IsTrue(b.IsRelative);
            Assert.AreEqual(string.Empty, b.PathRoot);
            Assert.AreEqual(@"dir1\dir2\file.txt", b.ToString());
        }

        [Test]
        public void ReadPathWithSlashes()
        {
            PathBuilder b = new PathBuilder(@"dir1/dir2/file.txt");
            Assert.AreEqual(3, b.PathDepth);
            Assert.IsTrue(b.IsRelative);
            Assert.AreEqual(string.Empty, b.PathRoot);
            Assert.AreEqual(@"dir1\dir2\file.txt", b.ToString());
        }

        [Test]
        public void ReadPathWithTrailingDirSeparator()
        {
            PathBuilder b = new PathBuilder(@"dir1\dir2\file.txt\");
            Assert.AreEqual(3, b.PathDepth);
            Assert.IsTrue(b.IsRelative);
            Assert.AreEqual(string.Empty, b.PathRoot);
            Assert.AreEqual(@"dir1\dir2\file.txt", b.ToString());
        }

        [Test]
        public void ReadAbsolutePathWithoutDisk()
        {
            PathBuilder b = new PathBuilder(@"\dir1\dir2\file.txt");
            Assert.AreEqual(3, b.PathDepth);
            Assert.IsFalse(b.IsRelative);
            Assert.AreEqual(@"\", b.PathRoot);
            Assert.AreEqual(@"\dir1\dir2\file.txt", b.ToString());
        }

        [Test]
        public void ReadAbsolutePathWithDisk()
        {
            PathBuilder b = new PathBuilder(@"D:\dir1\dir2\file.txt");
            Assert.AreEqual(3, b.PathDepth);
            Assert.IsFalse(b.IsRelative);
            Assert.AreEqual(@"D:\", b.PathRoot);
            Assert.AreEqual(@"D:\dir1\dir2\file.txt", b.ToString());
        }

        [Test]
        public void ReadUncPath()
        {
            PathBuilder b = new PathBuilder(@"\\servername.pvt\sharename\directory\file.txt");
            Assert.AreEqual(2, b.PathDepth);
            Assert.IsFalse(b.IsRelative);
            Assert.IsTrue(b.IsUncPath);
            Assert.AreEqual(@"\\servername.pvt\sharename", b.PathRoot);
            Assert.AreEqual(@"\\servername.pvt\sharename\directory\file.txt", b.ToString());
        }

        [Test]
        public void ReadUncPathWithRootOnly()
        {
            PathBuilder b = new PathBuilder(@"\\servername.pvt\sharename");
            Assert.AreEqual(0, b.PathDepth);
            Assert.IsFalse(b.IsRelative);
            Assert.IsTrue(b.IsUncPath);
            Assert.AreEqual(@"\\servername.pvt\sharename", b.PathRoot);
            Assert.AreEqual(@"\\servername.pvt\sharename", b.ToString());
        }

        [Test]
        public void AbsoluteToUnixPath()
        {
            PathBuilder b = new PathBuilder(@"\dir1\dir2\file.txt");
            Assert.AreEqual(@"/dir1/dir2/file.txt", b.ToUnixPath());
        }

        [Test]
        public void RelativeToUnixPath()
        {
            PathBuilder b = new PathBuilder(@"dir1\dir2\file.txt");
            Assert.AreEqual(@"dir1/dir2/file.txt", b.ToUnixPath());
        }

        [TestCase(true, "test", "test")]
        [TestCase(true, "test", "test/file.txt")]
        [TestCase(true, "/test", "/test/file.txt")]
        [TestCase(true, "d:/test", "d:/test/file.txt")]
        [TestCase(false, "/test", "test")]
        [TestCase(false, "/test/test2", "/test")]
        [TestCase(false, "test", "Test/file.txt")]
        public void IsBasePathOf_CaseSensitive(bool expectedResult, string basePath, string path)
        {
            Assert.AreEqual(expectedResult, new PathBuilder(basePath).IsBasePathOf(new PathBuilder(path), true));
        }

        [TestCase(true, "test", "test")]
        [TestCase(true, "test", "test/file.txt")]
        [TestCase(true, "/test", "/test/file.txt")]
        [TestCase(true, "d:/test", "d:/test/file.txt")]
        [TestCase(false, "/test", "test")]
        [TestCase(false, "/test/test2", "/test")]
        [TestCase(true, "test", "Test/file.txt")]
        public void IsBasePathOf_CaseInsensitive(bool expectedResult, string basePath, string path)
        {
            Assert.AreEqual(expectedResult, new PathBuilder(basePath).IsBasePathOf(new PathBuilder(path), false));
        }

        [TestCase("", "test", "test")]
        [TestCase("file.txt", "test", "test/file.txt")]
        [TestCase(null, "test", "test2/file.txt")]
        [TestCase(@"file.txt", @"d:\base", @"d:\base\file.txt")]
        [TestCase(@"file.txt", @"d:\base\", @"d:\base\file.txt")]
        [TestCase(@"test\file.txt", @"d:\base", @"d:\base\test\file.txt")]
        [TestCase(@"test\file.txt", @"d:\base", @"d:\base/test\file.txt")]
        [TestCase(@"test\file.txt", @"d:/base", @"d:\base\test\file.txt")]
        [TestCase(@"site.css", @"projDir", @"projDir\site.css")]
        public void DebasePath(string expectedPath, string basePath, string path)
        {
            PathBuilder pathBuilder = new PathBuilder(basePath).DebasePath(path, true);

            if (expectedPath == null)
                Assert.IsNull(pathBuilder);
            else
                Assert.AreEqual(expectedPath, pathBuilder.ToString());
        }

        [Test]
        public void CombinePaths()
        {
            PathBuilder path1 = new PathBuilder(@"dir1/dir2");
            PathBuilder path2 = new PathBuilder(@"dir3/dir4");
            PathBuilder combined = path1.CombineWith(path2);

            Assert.AreEqual(4, combined.PathDepth);
            Assert.IsTrue(combined.IsRelative);
            Assert.AreEqual(@"dir1\dir2\dir3\dir4", combined.ToString());
        }

        [Test]
        public void TryingToCombinePathWithAbsolutePathShouldRaiseError()
        {
            PathBuilder path1 = new PathBuilder(@"dir1/dir2");
            PathBuilder path2 = new PathBuilder(@"/dir3/dir4");
            var ex = Assert.Throws<ArgumentException>(delegate { path1.CombineWith(path2); });
            StringAssert.Contains("Cannot combine a path with an absolute path", ex.Message);
        }
    }
}