using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;

namespace LibroLib.FileSystem
{
    public class ApplicationInfo : IApplicationInfo
    {
        public string AppRootDirectory
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                if (appRootDirectory == null)
                {
                    string applicationFullPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                    string applicationDirectory = Path.GetDirectoryName(Path.GetFullPath(applicationFullPath));
                    return applicationDirectory;
                }

                return appRootDirectory;
            }

            set
            {
                appRootDirectory = value;
            }
        }

        public Version AppVersion
        {
            get
            {
                string executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
                if (executingAssemblyLocation == null)
                    throw new InvalidOperationException("Executing assembly location is null, cannot determine the AppVersion.");
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(executingAssemblyLocation);
                string fileVersion = version.FileVersion;
                return new Version(fileVersion);
            }
        }

        public string AppVersionString
        {
            get
            {
                Version version = AppVersion;

                if (version.Build == 0)
                    return version.ToString(2);

                return version.ToString(3);
            }
        }

        public bool IsMono
        {
            get
            {
                return Type.GetType("Mono.Runtime") != null;
            }
        }

        public string MonoVersion
        {
            get
            {
                return MonoVersionStatic;
            }
        }

        public bool Is64Bit
        {
            get { return IntPtr.Size > 4; }
        }

        public long MemoryUsed
        {
            get
            {
                if (IsMono)
                    return Process.GetCurrentProcess().PrivateMemorySize64;

                return Environment.WorkingSet;
            }
        }

        public long GCTotalMemory
        {
            get { return GC.GetTotalMemory(false); }
        }

        public static string MonoVersionStatic
        {
            get
            {
                Type type = Type.GetType("Mono.Runtime");
                if (type != null)
                {
                    MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
                    if (displayName != null)
                        return (string)displayName.Invoke(null, null);
                }

                return null;
            }
        }

        public string GetAppDirectoryPath(string subpath)
        {
            string path = Path.Combine(AppRootDirectory, subpath);
            return path;
        }

        private string appRootDirectory;
    }
}