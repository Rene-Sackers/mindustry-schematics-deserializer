using System.IO;

namespace MindustrySchematics.Deserializer.Extensions
{
	internal static class StreamExtensions
	{
		public static byte[] ReadBytes(this Stream stream, int length)
		{
			var buffer = new byte[length];
			stream.Read(buffer, 0, buffer.Length);

			return buffer;
		}
	}
}
