using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dokan;

namespace DokanRamDisk
{
	class Program
	{
		public enum FileSystemNodeType
		{
			File,
			Directory,
		}

		public class FileSystemNode
		{
			public string Name;
			public bool IsDirectory;
			public FileSystemNodeType Type;
			public List<FileSystemNode> Children;
		}

		public class DokanRamDisk : DokanOperations
		{
			FileSystemNode Root;


			int DokanOperations.CreateFile(string filename, System.IO.FileAccess access, System.IO.FileShare share, System.IO.FileMode mode, System.IO.FileOptions options, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.OpenDirectory(string filename, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.CreateDirectory(string filename, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.Cleanup(string filename, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.CloseFile(string filename, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.ReadFile(string filename, byte[] buffer, ref uint readBytes, long offset, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.WriteFile(string filename, byte[] buffer, ref uint writtenBytes, long offset, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.FlushFileBuffers(string filename, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.GetFileInformation(string filename, FileInformation fileinfo, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.FindFiles(string filename, IEnumerable<FileInformation> files, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.SetFileAttributes(string filename, System.IO.FileAttributes attr, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.SetFileTime(string filename, DateTime ctime, DateTime atime, DateTime mtime, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.DeleteFile(string filename, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.DeleteDirectory(string filename, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.MoveFile(string filename, string newname, bool replace, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.SetEndOfFile(string filename, long length, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.SetAllocationSize(string filename, long length, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.LockFile(string filename, long offset, long length, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.UnlockFile(string filename, long offset, long length, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.GetDiskFreeSpace(ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes, DokanFileInfo info)
			{
				throw new NotImplementedException();
			}

			int DokanOperations.Unmount(DokanFileInfo info)
			{
				throw new NotImplementedException();
			}
		}

		static void Main(string[] args)
		{
		}
	}
}
