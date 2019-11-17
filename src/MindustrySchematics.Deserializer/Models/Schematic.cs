using System.Collections.Generic;
using System.Linq;

namespace MindustrySchematics.Deserializer.Models
{
	public class Schematic
	{
		private const string NameTagKeyName = "name";

		public int Width { get; }

		public int Height { get; }

		public byte Version { get; }

		public string Name => Tags?.ContainsKey(NameTagKeyName) == true ? Tags[NameTagKeyName] : "Unknown";

		public IReadOnlyDictionary<string, string> Tags { get; }

		public IReadOnlyCollection<Tile> Tiles { get; }

		internal Schematic(byte version, int width, int height, Dictionary<string, string> tags, IEnumerable<Tile> tiles)
		{
			Version = version;
			Width = width;
			Height = height;
			Tags = tags;
			Tiles = tiles.ToList();
		}
	}
}