using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace LibroLib.FileSystem
{
    public class FileSet
    {
        public FileSet()
        {
        }

        public FileSet(string baseDir, IEnumerable<string> files)
        {
            Contract.Requires(files != null);

            this.baseDir = baseDir;
            this.files.AddRange(files);
        }

        public FileSet(IEnumerable<string> files)
            : this(null, files)
        {
            Contract.Requires(files != null);
        }

        public string BaseDir
        {
            get { return baseDir; }
            set { baseDir = value; }
        }

        public IList<string> Files
        {
            get { return files; }
        }

        public void AddFile(string fileName)
        {
            files.Add(fileName);
        }

        public void ClearList()
        {
            files.Clear();
        }

        public void SetFile(string fileName)
        {
            ClearList();
            files.Add(fileName);
            BaseDir = Path.GetDirectoryName(fileName);
        }

        public void SetFiles(string baseDir, IEnumerable<string> fileNames)
        {
            Contract.Requires(fileNames != null);
            ClearList();
            files.AddRange(fileNames);
            BaseDir = baseDir;
        }

        public void SortFiles()
        {
            files.Sort();
        }

        private string baseDir;
        private readonly List<string> files = new List<string>();
    }
}