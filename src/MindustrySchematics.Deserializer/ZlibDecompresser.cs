using System.IO;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace MindustrySchematics.Deserializer
{
	internal static class ZlibDecompresser
	{
		private static async Task CopyTo(Stream src, Stream dest)
		{
			var bytes = new byte[4096];

			int cnt;

			while ((cnt = await src.ReadAsync(bytes, 0, bytes.Length)) != 0)
				dest.Write(bytes, 0, cnt);
		}

		public static async Task<MemoryStream> Decompress(Stream stream)
		{
			var memoryStream = new MemoryStream();
			using var zlibStream = new ZlibStream(stream, CompressionMode.Decompress);

			await CopyTo(zlibStream, memoryStream);
			memoryStream.Seek(0, SeekOrigin.Begin);

			return memoryStream;
		}
	}
}