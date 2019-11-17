using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MindustrySchematics.Deserializer.ConsoleApp
{
	public class Program
	{
		private static async Task Main()
		{
			var schematicBase64 = GetFileBase64("./Schematics/test.msch");
			var schematic = await SchematicDeserializer.Deserialize(schematicBase64);

			var tagsString = string.Join("\n\t", schematic.Tags.Select(t => $"{t.Key} = {t.Value}").ToArray());
			var tilesString = string.Join("\n\t", schematic.Tiles.Select(t =>
				$"Type: {t.Type}\n" +
				$"\tConfig: {t.Config}\n" +
				$"\tX,Y: {t.X},{t.Y}\n" +
				$"\tRotation: {t.Rotation}\n").ToArray());

			Console.WriteLine(
				$"Name: {schematic.Name}\n" +
				$"Width: {schematic.Width}\n" +
				$"Height: {schematic.Height}\n" +
				$"Version: {schematic.Version}\n" +
				$"Block names:\n\t{string.Join("\n\t", schematic.BlockNames.ToArray())}\n" +
				$"Tags:\n\t{tagsString}\n" +
				$"Tiles:\n\t{tilesString}");

			Console.ReadKey();
		}

		private static string GetFileBase64(string path)
		{
			var bytes = File.ReadAllBytes(path);
			return Convert.ToBase64String(bytes);
		}
	}
}