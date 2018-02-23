using System;
using System.Diagnostics.Contracts;

namespace LibroLib.FileSystem
{
    [ContractClass(typeof(IApplicationInfoContract))]
    public interface IApplicationInfo
    {
        string AppRootDirectory { get; }

        Version AppVersion { get; }
        string AppVersionString { get; }

        bool IsMono { get; }
        string MonoVersion { get; }

        bool Is64Bit { get; }

        long MemoryUsed { get; }
        long GCTotalMemory { get; }

        string GetAppDirectoryPath(string subpath);
    }

    [ContractClassFor(typeof(IApplicationInfo))]
    // ReSharper disable once InconsistentNaming
    internal abstract class IApplicationInfoContract : IApplicationInfo
    {
        string IApplicationInfo.AppRootDirectory
        {
            get { throw new NotImplementedException(); }
        }

        Version IApplicationInfo.AppVersion
        {
            get
            {
                Contract.Ensures(Contract.Result<System.Version>() != null);
                throw new NotImplementedException();
            }
        }

        string IApplicationInfo.AppVersionString
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                throw new NotImplementedException();
            }
        }

        bool IApplicationInfo.IsMono
        {
            get { throw new NotImplementedException(); }
        }

        string IApplicationInfo.MonoVersion
        {
            get { throw new NotImplementedException(); }
        }

        bool IApplicationInfo.Is64Bit
        {
            get { throw new NotImplementedException(); }
        }

        long IApplicationInfo.MemoryUsed
        {
            get { throw new NotImplementedException(); }
        }

        long IApplicationInfo.GCTotalMemory
        {
            get { throw new NotImplementedException(); }
        }

        string IApplicationInfo.GetAppDirectoryPath(string subpath)
        {
            Contract.Ensures(Contract.Result<string>() != null);
            throw new NotImplementedException();
        }
    }
}