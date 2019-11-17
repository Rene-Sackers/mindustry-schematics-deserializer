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
			var schematic = await SchematicDeserializer.Deserialize(schematicBase64);

			var tagsString = string.Join("\n\t", schematic.Tags.Select(t => $"{t.Key} = {t.Value}").ToArray());
			var tilesString = string.Join("\n\t", schematic.Tiles.Select(t =>
				$"Block: {t.BlockName}\n" +
				$"\tConfig: {t.Config}\n" +
				$"\tX,Y: {t.X},{t.Y}\n" +
				$"\tRotation: {t.Rotation}\n").ToArray());

			Console.WriteLine(
				$"Name: {schematic.Name}\n" +
				$"Width: {schematic.Width}\n" +
				$"Height: {schematic.Height}\n" +
				$"Version: {schematic.Version}\n" +
				$"Tags:\n\t{tagsString}\n" +
				$"Tiles:\n\t{tilesString}");

			var atlas = SpriteAtlas.FromFile("Sprites/sprites.atlas");
			var atlasSpriteSet = atlas.SpriteSets["sprites.png"];

			Directory.CreateDirectory("sprites-render");

			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["titanium-conveyor-0-0"], "sprites-render/titanium-conveyor-0-0.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["armored-conveyor-0-1"], "sprites-render/armored-conveyor-0-1.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["armored-conveyor-1-1"], "sprites-render/armored-conveyor-1-1.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["armored-conveyor-2-1"], "sprites-render/armored-conveyor-2-1.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["armored-conveyor-3-1"], "sprites-render/armored-conveyor-3-1.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["armored-conveyor-4-1"], "sprites-render/armored-conveyor-4-1.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["pulse-conduit-top-0"], "sprites-render/pulse-conduit-top-0.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["pulse-conduit-top-1"], "sprites-render/pulse-conduit-top-1.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["pulse-conduit-top-2"], "sprites-render/pulse-conduit-top-2.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["pulse-conduit-top-3"], "sprites-render/pulse-conduit-top-3.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["block-1"], "sprites-render/block-1.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["block-2"], "sprites-render/block-2.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["block-3"], "sprites-render/block-3.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["block-pulse-conduit-full"], "sprites-render/block-pulse-conduit-full.png");
			//SchematicVisualizer.RenderSprite(atlasSpriteSet.Sprites["pulse-conduit-top-0"], "sprites-render/pulse-conduit-top-0.png");

			SchematicVisualizer.Visualize(schematic, atlas, "sprites-render/schematic.png");

			Console.ReadKey();
		}

		private static string GetFileBase64(string path)
		{
			var bytes = File.ReadAllBytes(path);
			return Convert.ToBase64String(bytes);
		}
	}
}