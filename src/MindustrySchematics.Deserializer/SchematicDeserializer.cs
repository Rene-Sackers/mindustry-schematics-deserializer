using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MindustrySchematics.Deserializer.Extensions;
using MindustrySchematics.Deserializer.Helpers;
using MindustrySchematics.Deserializer.Models;

namespace MindustrySchematics.Deserializer
{
	public class SchematicDeserializer
	{
		private static readonly byte[] Header = Encoding.UTF8.GetBytes("msch");

		public static Task<Schematic> Deserialize(string base64)
		{
			byte[] bytes;
			try
			{
				bytes = Convert.FromBase64String(base64);
			}
			catch (Exception ex)
			{
				throw new DeserializationException(DeserializationExceptionReason.InvalidBase64, "Could not get base64 bytes from given string.", ex);
			}

			return Deserialize(bytes);
		}

		public static async Task<Schematic> Deserialize(byte[] bytes)
		{
			using var memoryStream = new MemoryStream(bytes);

			return await Deserialize(memoryStream);
		}

		public static async Task<Schematic> Deserialize(Stream stream)
		{
			if (!stream.CanRead || !stream.CanSeek)
				throw new InvalidOperationException("A readable and seekable stream is required to deserialize a schematic.");

			if (stream.Length < Header.Length)
				throw new DeserializationException(DeserializationExceptionReason.MissingMschHeader, "Invalid schematic, stream smaller than first header bytes");

			var header = stream.ReadBytes(Header.Length);
			if (!header.SequenceEqual(Header))
				throw new DeserializationException(DeserializationExceptionReason.MissingMschHeader, "Invalid schematic, did not find expected header bytes.");

			var version = (byte)stream.ReadByte();

			var decompressed = await ZlibDecompresser.Decompress(stream);
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
			var blockNames = new string[blockCount];
			for (var i = 0; i < blockCount; i++)
			{
				var blockName = inflater.ReadUTF() ?? "air";
				blockNames[i] = blockName;
			}

			var tileCount = inflater.ReadInt();
			var tiles = new List<Tile>();
			for (var i = 0; i < tileCount; i++)
			{
				var blockIndex = inflater.ReadByte();
				var blockName = blockNames[blockIndex];
				var (x, y) = GetPosition(inflater.ReadInt(), height);
				var config = inflater.ReadInt();
				var rotation = inflater.ReadByte();

				tiles.Add(new Tile(blockName, x, y, config, rotation));
			}

			return new Schematic(version, width, height, tags, tiles);
		}

		private static (int X, int Y) GetPosition(int position, int schematicHeight)
		{
			var x = MindustryPositionHelper.X(position);
			var y = MindustryPositionHelper.Y(position);

			// For some reason, Y is flipped vertically :\
			y = (short)(schematicHeight - 1 - y);

			return (x, y);
		}
	}
}