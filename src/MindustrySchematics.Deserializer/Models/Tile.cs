using MindustrySchematics.Deserializer.Helpers;

namespace MindustrySchematics.Deserializer.Models
{
	public class Tile
	{
		public string BlockName { get; }

		public int X { get; set; }

		public int Y { get; set; }

		public int Config { get; set; }

		public byte Rotation { get; set; }

		internal Tile(string blockName, int position, int config, byte rotation)
		{
			X = MindustryPositionHelper.X(position);
			Y = MindustryPositionHelper.Y(position);
			BlockName = blockName;
			Config = config;
			Rotation = rotation;
		}
	}
}