using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpUtils.Streams;
using Dokan;
using DokanRamDisk.Dokan;

namespace DokanRamDisk
{
	class Program
	{
		public enum FileSystemNodeType
		{
			File,
			Directory,
		}

		public class FileSystemNode : IEnumerable<FileSystemNode>, IDokanFile
		{
			public string Name { get; private set; }
			public bool IsDirectory { get { return Type == FileSystemNodeType.Directory; } }
			public bool IsFile { get { return Type == FileSystemNodeType.File; } }
			public FileSystemNodeType Type;
			public FileSystemNode Parent;
			public FileSystemNode Root
			{
				get
				{
					return (Parent != null) ? Parent.Root : this;
				}
			}
			public Dictionary<string, FileSystemNode> Children = new Dictionary<string, FileSystemNode>();
			private Stream _PrivateStream;
			private Stream PrivateStream
			{
				get
				{
					if (_PrivateStream == null) _PrivateStream = new MemoryStream();
					return new UncloseableStream(_PrivateStream);
				}
			}
			public DateTime CreationTime;
			public DateTime LastAccessTime;
			public DateTime LastWriteTime;
			public FileAttributes FileAttributes;


			public FileSystemNode(FileSystemNode Parent, string Name)
			{
				this.Parent = Parent;
				this.Name = Name;
				this.Type = FileSystemNodeType.Directory;
				if (Parent != null)
				{
					Parent.Children.Add(GetKeyName(Name), this);
				}

				CreationTime = DateTime.UtcNow;
				LastAccessTime = DateTime.UtcNow;
				LastWriteTime = DateTime.UtcNow;
			}

			public void Remove()
			{
				if (Parent != null)
				{
					Parent.Children.Remove(GetKeyName(this.Name));
					Parent = null;
				}
			}

			static private string GetKeyName(string Name)
			{
				return Name.ToLowerInvariant();
			}

			public bool Exists(string Path)
			{
				return TryAccess(Path, false) != null;
			}

			public FileSystemNode TryAccess(string Path, bool Create = false)
			{
				var Current = this;
				if (Path.StartsWith("\\"))
				{
					Path = Path.TrimStart('\\');
					Current = Root;
				}
				if (Path.Length > 0)
				{
					foreach (var Part in Path.Split('\\'))
					{
						var PartKey = GetKeyName(Part);

						if (!Current.Children.ContainsKey(PartKey))
						{
							if (!Create)
							{
								//throw(new KeyNotFoundException(String.Format("Can't access path '{0}'", Path)));
								return null;
							}
							var Item = new FileSystemNode(Current, Part);
						}

						Current = Current.Children[PartKey];
					}
				}
				return Current;
			}

			public FileSystemNode Access(string Path)
			{
				var Result = TryAccess(Path, Create: false);
				if (Result == null) throw (new System.IO.FileNotFoundException(String.Format("Can't access path '{0}'", Path)));
				return Result;
			}

			public FileSystemNode Create(string Path, FileSystemNodeType Type)
			{
				var Result = TryAccess(Path, Create: true);
				if (Result == null) throw (new System.IO.FileNotFoundException(String.Format("Can't access path '{0}'", Path)));
				Result.Type = Type;
				return Result;
			}

			public FileInformation FileInformation
			{
				get
				{
					return new FileInformation()
					{
						FileName = this.Name,
						Attributes = ((Type == FileSystemNodeType.Directory) ? FileAttributes.Directory : FileAttributes.Normal) | FileAttributes,
						CreationTime = CreationTime,
						LastAccessTime = LastAccessTime,
						LastWriteTime = LastWriteTime,
						Length = PrivateStream.Length,
					};
				}
			}

			IEnumerator<FileSystemNode> IEnumerable<FileSystemNode>.GetEnumerator()
			{
				return Children.Values.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return Children.Values.GetEnumerator();
			}

			public Stream Stream
			{
				get
				{
					return PrivateStream;
				}
			}
		}

		public class DokanRamDisk : DokanFileSystem
		{
			FileSystemNode Root;

			public DokanRamDisk()
			{
				Root = new FileSystemNode(null, "");
			}


			public override IDokanFile CreateFile(string filename, System.IO.FileAccess access, System.IO.FileShare share, System.IO.FileMode mode, System.IO.FileOptions options)
			{
				Log("CreateFile: {0}", filename);

				switch (mode)
				{
					case FileMode.Append:
						throw (new NotImplementedException());
					case FileMode.Create:
					case FileMode.CreateNew:
						if (mode == FileMode.CreateNew && Root.Exists(filename))
						{
							throw(new System.IO.IOException());
						}
						return Root.Create(filename, FileSystemNodeType.File);
					case FileMode.Open:
						return Root.Access(filename);
					case FileMode.OpenOrCreate:
						throw (new NotImplementedException());
					case FileMode.Truncate:
						throw (new NotImplementedException());
					default:
						throw (new NotImplementedException());
				}
			}

			public override void OpenDirectory(string filename, DokanFileInfo info)
			{
				Log("OpenDirectory: {0}", filename);
			}

			public override void CreateDirectory(string filename, DokanFileInfo info)
			{
				Log("CreateDirectory: {0}", filename);

				var Node = Root.Create(filename, FileSystemNodeType.Directory);
			}

			public override void Cleanup(string filename, DokanFileInfo info)
			{
				Log("Cleanup: {0}", filename);

				//throw new NotImplementedException();
			}

			public override FileInformation GetFileInformation(string filename, DokanFileInfo info)
			{
				Log("GetFileInformation: {0}", filename);

				return Root.Access(filename).FileInformation;
			}

			public override void SetFileAttributes(string filename, System.IO.FileAttributes attr, DokanFileInfo info)
			{
				Log("SetFileAttributes: {0}", filename);

				Root.Access(filename).FileAttributes = attr;
			}

			public override void SetFileTime(string filename, DateTime ctime, DateTime atime, DateTime mtime, DokanFileInfo info)
			{
				Log("SetFileTime: {0}", filename);

				var Node = Root.Access(filename);

				Node.CreationTime = ctime;
				Node.LastAccessTime = atime;
				Node.LastWriteTime = mtime;
			}

			public override void DeleteFile(string filename, DokanFileInfo info)
			{
				Log2("DeleteFile: {0}", filename);

				var Item = Root.Access(filename);
				if (!Item.IsFile) throw (new FileNotFoundException());
				Item.Remove();
			}

			public override void DeleteDirectory(string filename, DokanFileInfo info)
			{
				Log("DeleteDirectory: {0}", filename);

				var Item = Root.Access(filename);
				if (!Item.IsDirectory) throw(new DirectoryNotFoundException());
				Item.Remove();
			}

			public override void MoveFile(string filename, string newname, bool replace, DokanFileInfo info)
			{
				Log("MoveFile: {0}", filename);

				var Node = Root.Access(filename);
				Node.Remove();
				throw new NotImplementedException();
			}

			public override void SetEndOfFile(string filename, long length, DokanFileInfo info)
			{
				Log("SetEndOfFile: {0}", filename);

				throw new NotImplementedException();
			}

			public override void SetAllocationSize(string filename, long length, DokanFileInfo info)
			{
				Log("SetAllocationSize: {0}", filename);

				throw new NotImplementedException();
			}

			public override void GetDiskFreeSpace(ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes, DokanFileInfo info)
			{
				Log("GetDiskFreeSpace");

				freeBytesAvailable = 512 * 1024 * 1024;
				totalBytes = 512 * 1024 * 1024;
				totalFreeBytes = 512 * 1024 * 1024;
			}

			public override void Unmount(DokanFileInfo info)
			{
				Log("Unmount");

				//throw new NotImplementedException();
			}

			public override IEnumerable<FileInformation> FindFilesWithPattern(string filename, Wildcard searchPattern, DokanFileInfo info)
			{
				Log("UnlockFile: {0} : {1}", filename, searchPattern);

				foreach (var Item in Root.Access(filename))
				{
					Log("::::::::: {0}", Item.Name);
					if (searchPattern.IsMatch(Item.Name))
					{
						yield return Item.FileInformation;
					}
				}
			}
		}

		static void Main(string[] args)
		{
			DokanNet.DokanMain(new DokanOptions()
			{
				DebugMode = true,
				MountPoint = "Z:",
				NetworkDrive = false,
				RemovableDrive = true,
			}, new DokanRamDisk());
		}
	}
}
