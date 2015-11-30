using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Text;

namespace LibroLib.FileSystem
{
    public class PathBuilder
    {
        public PathBuilder(string path)
        {
            AnalyzePath(path);
        }

        public int PathDepth
        {
            get { return pathComponents.Count; }
        }

        public bool IsRelative
        {
            get { return pathRoot.Length == 0; }
        }

        public string PathRoot
        {
            get { return pathRoot; }
        }

        public bool IsUncPath
        {
            get { return pathRoot.StartsWith(@"\\", StringComparison.OrdinalIgnoreCase); }
        }

        public string PathX
        {
            get { return ToString(); }
        }

        public PathBuilder CombineWith (PathBuilder path)
        {
            Contract.Requires(path != null);

            if (!path.IsRelative)
                throw new ArgumentException ("Cannot combine a path with an absolute path", "path");

            PathBuilder combined = new PathBuilder(this);
            combined.pathComponents.AddRange(path.pathComponents);
            return combined;
        }

        public bool IsBasePathOf (PathBuilder other, bool caseSensitivePaths)
        {
            Contract.Requires(other != null);

            if (IsRelative != other.IsRelative)
                return false;

            if (IsUncPath != other.IsUncPath)
                return false;

            if (PathDepth > other.PathDepth)
                return false;

            StringComparison stringComparison = caseSensitivePaths ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            if (!PathRoot.Equals(other.PathRoot, stringComparison))
                return false;

            for (int i = 0; i < pathComponents.Count; i++)
            {
                string thisPathComponent = pathComponents[i];
                string otherPathComponent = other.pathComponents[i];
                if (!thisPathComponent.Equals(otherPathComponent, stringComparison))
                    return false;
            }

            return true;
        }

        public PathBuilder DebasePath (string path, bool caseSensitivePaths)
        {
            PathBuilder otherPathBuilder = new PathBuilder(path);
            return DebasePath(otherPathBuilder, caseSensitivePaths);
        }

        public PathBuilder DebasePath (PathBuilder path, bool caseSensitivePaths)
        {
            Contract.Requires (path != null);

            if (!IsBasePathOf (path, caseSensitivePaths))
                return null;

            PathBuilder debasedPath = new PathBuilder();

            for (int i = PathDepth; i < path.PathDepth; i++)
                debasedPath.pathComponents.Add (path.pathComponents[i]);

            return debasedPath;
        }

        public override string ToString ()
        {
            return ToStringInternal(Path.DirectorySeparatorChar);
        }

        public string ToUnixPath ()
        {
            return ToStringInternal('/');
        }

        private string ToStringInternal(char directorySeparatorChar)
        {
            StringBuilder s = new StringBuilder();
            char? separator = null;

            if (pathRoot == @"\" && directorySeparatorChar == '/')
                s.Append(directorySeparatorChar);
            else
                s.Append(pathRoot);

            if (IsUncPath)
                separator = directorySeparatorChar;

            foreach (string pathComponent in pathComponents)
            {
                s.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", separator, pathComponent);
                separator = directorySeparatorChar;
            }

            return s.ToString();
        }

        private void AnalyzePath(string path)
        {
            pathRoot = Path.GetPathRoot(path);

            // NOTE: "!path.Equals (pathRoot)" this is special handling needed for UNC paths
            while (!string.IsNullOrEmpty (path) && !path.Equals (pathRoot))
            {
                string pathComponent = Path.GetFileName (path);

                if (pathComponent.Length > 0)
                    pathComponents.Insert (0, pathComponent);

                path = Path.GetDirectoryName (path);

                if (path.Equals(pathRoot))
                    break;
            }
        }

        private PathBuilder ()
        {
        }

        private PathBuilder (PathBuilder copyFrom)
        {
            Contract.Requires (copyFrom != null);

            pathRoot = copyFrom.pathRoot;
            pathComponents.AddRange(copyFrom.pathComponents);
        }

        private string pathRoot = string.Empty;
        private readonly List<string> pathComponents = new List<string>();
    }
}