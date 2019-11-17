namespace MindustrySchematics.Deserializer.Helpers
{
	internal static class MindustryPositionHelper
	{
		/// <summary>
		/// Returns the X position from a Mindustry position value, usually deserialized from schematics.
		/// </summary>
		/// <param name="pos">Mindustry position value</param>
		/// <returns></returns>
		public static short X(int pos)
		{
			return (short)(pos >> 16);
		}

		/// <summary>
		/// Returns the Y position from a Mindustry position value, usually deserialized from schematics.
		/// </summary>
		/// <param name="pos">Mindustry position value</param>
		/// <returns></returns>
		public static short Y(int pos)
		{
			return (short)(pos & 0xFFFF);
		}
	}
}