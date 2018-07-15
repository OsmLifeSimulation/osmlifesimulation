using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Global
{
	public static class Globals
	{
		public const int blockSize = 50;
		public static Random rnd = new Random();
		public static BinaryFormatter formatter = new BinaryFormatter();

		public static void Clear(this MemoryStream source)
		{
			byte[] buffer = source.GetBuffer();
			Array.Clear(buffer, 0, buffer.Length);
			source.Position = 0;
			source.SetLength(0);
		}
	}
}
