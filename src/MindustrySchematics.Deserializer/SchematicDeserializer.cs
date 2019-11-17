using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MindustrySchematics.Deserializer.Extensions;
using MindustrySchematics.Deserializer.Models;

namespace MindustrySchematics.Deserializer
{
	public class SchematicDeserializer
	{
		private static readonly byte[] Header = Encoding.UTF8.GetBytes("msch");

		public static async Task<Schematic> Deserialize(string base64)
		{
			var bytes = Convert.FromBase64String(base64);
			using var memoryStream = new MemoryStream(bytes);

			var header = memoryStream.ReadBytes(Header.Length);
			if (!header.SequenceEqual(Header))
			{
				throw new InvalidOperationException("Invalid schematic, did not find expected header bytes.");
			}

			var version = (byte) memoryStream.ReadByte();

			var decompressed = await ZlibDecompresser.Decompress(memoryStream);
			var inflater = new InflaterInputStream(decompressed);

			var width = inflater.ReadShort();
			var height = inflater.ReadShort();

			var tagCount = inflater.ReadByte();
			var tags = new Dictionary<string, string>(tagCount);
			for (var i = 0; i < tagCount; i++)
			{
				tags.Add(inflater.ReadUTF(), inflater.ReadUTF());
			}

			var blockCount = inflater.ReadByte();
			var blockNames = new List<string>(blockCount);
			for (var i = 0; i < blockCount; i++)
			{
				var blockName = inflater.ReadUTF() ?? "air";
				blockNames.Add(blockName);
			}

			var tileCount = inflater.ReadInt();
			var tiles = new List<Tile>();
			for (var i = 0; i < tileCount; i++)
			{
				var type = inflater.ReadByte();
				var position = inflater.ReadInt();
				var config = inflater.ReadInt();
				var rotation = inflater.ReadByte();

				tiles.Add(new Tile(type, position, config, rotation));
			}

			return new Schematic(version, width, height, tags, blockNames, tiles);
		}
	}
}