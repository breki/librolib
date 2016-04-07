using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using LibroLib.FileSystem;
using NUnit.Framework;

namespace LibroLib.Tests
{
    public class TestApplicationInfo : IApplicationInfo
    {
        public string AppRootDirectory
        {
            get
            {
                return appRootDirectoryOverride ?? TestContext.CurrentContext.TestDirectory;
            }

            set
            {
                if (Path.IsPathRooted (value))
                    appRootDirectoryOverride = value;
                else
                {
                    string currentAppRootDirectory = AppRootDirectory;
                    appRootDirectoryOverride = Path.Combine (currentAppRootDirectory, value);
                }
            }
        }

        public Version AppVersion
        {
            get
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo (Assembly.GetExecutingAssembly ().Location);
                string fileVersion = version.FileVersion;
                return new Version (fileVersion);
            }
        }

        public string AppVersionString
        {
            get
            {
                Version version = AppVersion;

                if (version.Build == 0)
                    return version.ToString (2);

                return version.ToString (3);
            }
        }

        public bool IsMono
        {
            get { throw new System.NotImplementedException (); }
        }

        public string MonoVersion
        {
            get { throw new System.NotImplementedException (); }
        }

        public bool Is64Bit
        {
            get { throw new System.NotImplementedException (); }
        }

        public long MemoryUsed
        {
            get { throw new System.NotImplementedException (); }
        }

        public long GCTotalMemory
        {
            get { throw new System.NotImplementedException (); }
        }

        public string GetAppDirectoryPath (string subpath)
        {
            string path = Path.Combine (AppRootDirectory, subpath);
            return path;
        }

        private string appRootDirectoryOverride;
    }
}