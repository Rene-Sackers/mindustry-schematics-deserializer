using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MindustrySchematics.Deserializer.Models;

namespace MindustrySchematics.Deserializer.ConsoleApp
{
	public class Program
	{
		private static async Task Main()
		{
			var schematicBase64 = GetFileBase64("./Schematics/test.msch");
			var schematicFromBase64 = await SchematicDeserializer.Deserialize(schematicBase64);

			await using var schematicStream = File.OpenRead("./Schematics/test.msch");
			var schematicFromFileStream = await SchematicDeserializer.Deserialize(schematicStream);

			var schematicBytes = File.ReadAllBytes("./Schematics/test.msch");
			var schematicFromBytes = await SchematicDeserializer.Deserialize(schematicBytes);

			var tagsString = string.Join("\n\t", schematicFromBase64.Tags.Select(t => $"{t.Key} = {t.Value}").ToArray());
			var tilesString = string.Join("\n\t", schematicFromBase64.Tiles.Select(t =>
				$"Block: {t.BlockName}\n" +
				$"\tConfig: {t.Config}\n" +
				$"\tX,Y: {t.X},{t.Y}\n" +
				$"\tRotation: {t.Rotation}\n").ToArray());

			Console.WriteLine(
				$"Name: {schematicFromBase64.Name}\n" +
				$"Width: {schematicFromBase64.Width}\n" +
				$"Height: {schematicFromBase64.Height}\n" +
				$"Version: {schematicFromBase64.Version}\n" +
				$"Tags:\n\t{tagsString}\n" +
				$"Tiles:\n\t{tilesString}");

			var atlas = SpriteAtlas.FromFile("Sprites/sprites.atlas");
			var atlasSpriteSet = atlas.SpriteSets["sprites.png"];

			Directory.CreateDirectory("sprites-render");
			SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["titanium-conveyor-0-0"], "sprites-render/titanium-conveyor-0-0.png");

			SchematicVisualizer.SaveToFile(schematicFromBase64, atlas, "sprites-render/schematic-read-base64.png");
			SchematicVisualizer.SaveToFile(schematicFromFileStream, atlas, "sprites-render/schematic-read-stream.png");
			SchematicVisualizer.SaveToFile(schematicFromBytes, atlas, "sprites-render/schematic-read-bytes.png");

			await using var stream = SchematicVisualizer.RenderToStream(schematicFromBase64, atlas);
			await using var file = File.Create("sprites-render/schematic-render-stream.png");
			await stream.CopyToAsync(file);
			await file.FlushAsync();
			file.Close();
		}

		private static string GetFileBase64(string path)
		{
			var bytes = File.ReadAllBytes(path);
			return Convert.ToBase64String(bytes);
		}
	}
}