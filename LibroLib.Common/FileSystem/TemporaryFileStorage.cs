using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using LibroLib.Misc;

namespace LibroLib.FileSystem
{
    public class TemporaryFileStorage : ITemporaryFileStorage
    {
        public TemporaryFileStorage(
            IApplicationInfo applicationInfo,
            IFileSystem fileSystem,
            ITimeService timeService)
        {
            this.applicationInfo = applicationInfo;
            this.fileSystem = fileSystem;
            this.timeService = timeService;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Clear()
        {
            try
            {
                fileSystem.DeleteDirectory(TempDirPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not delete the temporary file storage directory '{0}': {1}".Fmt(TempDirPath, ex));
            }
        }

        public string CreateTempDirectory(string prefix)
        {
            string fullPath;

            while (true)
            {
                string name = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}",
                    prefix,
                    random.Next(1000, 9000));

                fullPath = Path.Combine(TempDirPath, name);
                if (!fileSystem.DoesDirectoryExist(fullPath))
                    break;
            }

            return fullPath;
        }

        public void DeleteOldStuff(TimeSpan minimumAge)
        {
            DeleteOldStuffFrom(TempDirPath, minimumAge);
        }

        public string GetTemporaryFilePath(string fileName)
        {
            string tempDir = EnsureTempDirExists();
            return Path.Combine(tempDir, fileName);
        }

        public string GetTemporaryFilePathWithTimestamp(string fileNameFormat)
        {
            string fileName = string.Format(
                CultureInfo.InvariantCulture,
                fileNameFormat,
                timeService.CurrentTime.Ticks);
            return GetTemporaryFilePath(fileName);
        }

        public bool IsInTempStorage(string fileName)
        {
            return fileName.StartsWith(TempDirPath, StringComparison.OrdinalIgnoreCase);
        }

        private string TempDirPath
        {
            get { return applicationInfo.GetAppDirectoryPath("Temp"); }
        }

        private bool DeleteOldStuffFrom(string directoryName, TimeSpan minimumAge)
        {
            Contract.Requires(directoryName != null);

            if (!fileSystem.DoesDirectoryExist(directoryName))
                return true;

            bool someFilesRemained = false;

            // 28.03.2012: we no longer recursively call DeleteOldStuffFrom, we only check
            // file ages at the root level to prevent update package DLLs from being deleted
            foreach (IDirectoryInformation dirInfo in fileSystem.GetDirectorySubdirectories(directoryName))
            {
                //if (log.IsDebugEnabled)
                //    log.DebugFormat ("Directory '{0}', last write '{1}'", dirInfo.FullName, dirInfo.LastWriteTimeUtc);

                if (IsFileEntryTooOld(dirInfo, minimumAge))
                    fileSystem.DeleteDirectory(dirInfo.FullName);
                else
                    someFilesRemained = true;
            }

            foreach (IFileInformation file in fileSystem.GetDirectoryFiles(directoryName))
            {
                //if (log.IsDebugEnabled)
                //    log.DebugFormat("File '{0}', last write '{1}'", file.FullName, file.LastWriteTimeUtc);

                if (IsFileEntryTooOld(file, minimumAge))
                    fileSystem.DeleteFile(file.FullName, true);
                else
                    someFilesRemained = true;
            }

            return !someFilesRemained;
        }

        private bool IsFileEntryTooOld(IFileEntryInformation fileEntry, TimeSpan minimumAge)
        {
            Contract.Requires(fileEntry != null);

            if ((timeService.CurrentTimeUtc - fileEntry.CreationTimeUtc) >= minimumAge)
                return true;

            if ((timeService.CurrentTimeUtc - fileEntry.LastWriteTimeUtc) >= minimumAge)
                return true;

            return false;
        }

        private string EnsureTempDirExists()
        {
            string tempDir = TempDirPath;
            fileSystem.EnsureDirectoryExists(tempDir);
            return tempDir;
        }

        private readonly IApplicationInfo applicationInfo;
        private readonly IFileSystem fileSystem;
        //private static readonly ILog log = LogManager.GetLogger(typeof(TemporaryFileStorage));
        private readonly ITimeService timeService;
        private readonly Random random = new Random();
    }
}