using System.IO;

namespace MindustrySchematics.Deserializer.Extensions
{
	internal static class MemoryStreamExtensions
	{
		public static byte[] ReadBytes(this MemoryStream memoryStream, int length)
		{
			var buffer = new byte[length];
			memoryStream.Read(buffer, 0, buffer.Length);

			return buffer;
		}
	}
}
