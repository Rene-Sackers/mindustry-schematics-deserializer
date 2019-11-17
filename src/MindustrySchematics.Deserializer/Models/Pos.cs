namespace MindustrySchematics.Deserializer.Models
{
	internal static class Pos
	{
		/** Returns packed position from an x/y position. The values must be within short limits. */
		public static int Get(int x, int y)
		{
			return ((short)x << 16) | ((short)y & 0xFFFF);
		}

		/** Returns the x component of a position. */
		public static short X(int pos)
		{
			return (short)(pos >> 16);
		}

		/** Returns the y component of a position. */
		public static short Y(int pos)
		{
			return (short)(pos & 0xFFFF);
		}
	}
}