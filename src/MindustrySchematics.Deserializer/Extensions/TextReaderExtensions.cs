using System.IO;

namespace MindustrySchematics.Deserializer.Extensions
{
	public static class TextReaderExtensions
	{
		public static string ReadUntilLineStartsWith(this TextReader reader, string startsWith)
		{
			string line;
			do
			{
				line = reader.ReadLine();
			} while (!line.StartsWith(startsWith));

			return line;
		}
	}
}
