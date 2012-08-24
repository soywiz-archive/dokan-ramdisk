using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Dokan
{
    public class DokanFileInfo
    {
        public Object Context;
        public bool IsDirectory;
        public ulong InfoId;
        public uint ProcessId;
        public bool DeleteOnClose;
        public bool PagingIo;
        public bool SynchronousIo;
        public bool Nocache;
        public bool WriteToEndOfFile;
        public readonly ulong DokanContext; // for internal use

        public DokanFileInfo(ulong dokanContext)
        {
            Context = null;
            IsDirectory = false;
            DeleteOnClose = false;
            PagingIo = false;
            SynchronousIo = false;
            Nocache = false;
            WriteToEndOfFile = false;
            InfoId = 0;
            DokanContext = dokanContext;
        }
    }


    public class FileInformation
    {
        public FileAttributes Attributes;
        public DateTime CreationTime;
        public DateTime LastAccessTime;
        public DateTime LastWriteTime;
        public long Length;
        public string FileName;
        /*
        public FileInformation()
        {
            Attributes = FileAttributes.Normal;
            Length = 0;
        }
         */
    }

	public interface IDokanFile
	{
		Stream Stream { get; }
	}

    abstract public class DokanFileSystem
    {

		protected void Log(string Format, params object[] Params)
		{
#if DEBUG
			//Console.WriteLine(Format, Params);
#endif
		}

		protected void Log2(string Format, params object[] Params)
		{
#if DEBUG
			Console.WriteLine(Format, Params);
#endif
		}

        abstract public IDokanFile CreateFile(string filename, FileAccess access, FileShare share, FileMode mode, FileOptions options);
		abstract public void OpenDirectory(string filename, DokanFileInfo info);
		abstract public void CreateDirectory(string filename, DokanFileInfo info);
		abstract public void Cleanup(string filename, DokanFileInfo info);
		//abstract public int CloseFile(string filename, DokanFileInfo info);
		//abstract public int ReadFile(string filename, byte[] buffer, ref uint readBytes, long offset, DokanFileInfo info);
		//abstract public int WriteFile(string filename, byte[] buffer, ref uint writtenBytes, long offset, DokanFileInfo info);
		//abstract public int FlushFileBuffers(string filename, DokanFileInfo info);
		abstract public FileInformation GetFileInformation(string filename, DokanFileInfo info);
		abstract public IEnumerable<FileInformation> FindFilesWithPattern(string filename, Wildcard searchPattern, DokanFileInfo info);
		abstract public void SetFileAttributes(string filename, FileAttributes attr, DokanFileInfo info);
		abstract public void SetFileTime(string filename, DateTime ctime, DateTime atime, DateTime mtime, DokanFileInfo info);
		abstract public void DeleteFile(string filename, DokanFileInfo info);
		abstract public void DeleteDirectory(string filename, DokanFileInfo info);
		abstract public void MoveFile(string filename, string newname, bool replace, DokanFileInfo info);
		abstract public void SetEndOfFile(string filename, long length, DokanFileInfo info);
		abstract public void SetAllocationSize(string filename, long length, DokanFileInfo info);
		virtual public void LockFile(string filename, long offset, long length, DokanFileInfo info)
		{
			Log("LockFile: {0}", filename);
		}
		virtual public void UnlockFile(string filename, long offset, long length, DokanFileInfo info)
		{
			Log("UnlockFile: {0}", filename);
		}
		abstract public void GetDiskFreeSpace(ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes, DokanFileInfo info);
		abstract public void Unmount(DokanFileInfo info);
    }
}
