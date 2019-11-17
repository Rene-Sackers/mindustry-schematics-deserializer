﻿namespace MindustrySchematics.Deserializer.Models
{
	public class Tile
	{
		public int Type { get; set; }

		public int X { get; set; }

		public int Y { get; set; }

		public int Config { get; set; }

		public byte Rotation { get; set; }

		internal Tile(int type, int position, int config, byte rotation)
		{
			Type = type;
			X = Pos.X(position);
			Y = Pos.Y(position);
			Config = config;
			Rotation = rotation;
		}
	}
}