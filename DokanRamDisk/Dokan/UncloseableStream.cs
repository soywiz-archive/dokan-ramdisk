using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpUtils.Streams;

namespace DokanRamDisk.Dokan
{
	public class UncloseableStream : ProxyStream
	{
		public UncloseableStream(Stream Stream)
			: base(Stream)
		{
		}

		public override void Close()
		{
		}
	}
}
