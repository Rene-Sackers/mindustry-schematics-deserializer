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

		private static readonly Dictionary<string, (short x, short y)> BlockPositionModifiers = new Dictionary<string, (short x, short y)>
		{
			//{ "thorium-reactor", (-1, -1) },
			//{ "distributor", (0, -1) },
			//{ "distributor", (0, -1) },
			//{ "mass-driver", (0, -1) },
		};

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
				var (x, y) = GetPosition(blockName, inflater.ReadInt(), height);
				var config = inflater.ReadInt();
				var rotation = inflater.ReadByte();

				tiles.Add(new Tile(blockName, x, y, config, rotation));
			}

			return new Schematic(version, width, height, tags, tiles);
		}

		private static (int X, int Y) GetPosition(string blockName, int position, int schematicHeight)
		{
			var x = MindustryPositionHelper.X(position);
			var y = MindustryPositionHelper.Y(position);

			// For some reason, Y is flipped vertically :\
			y = (short)(schematicHeight - 1 - y);

			if (!BlockPositionModifiers.ContainsKey(blockName))
				return (x, y);

			var (xModifier, yModifier) = BlockPositionModifiers[blockName];
			x += xModifier;
			y += yModifier;

			return (x, y);
		}
	}
}