using System.Collections.Generic;
using System.IO;
using MindustrySchematics.Deserializer.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace MindustrySchematics.Deserializer
{
	public class SchematicVisualizer
	{
		private const int PixelsPerTile = 32;

		private static readonly Dictionary<string, string[]> BlockRenderModifiers = new Dictionary<string, string[]>
		{
			{"conveyor", new [] { "conveyor-0-0" }},
			{"titanium-conveyor", new [] { "titanium-conveyor-0-0" }},
			{"pulse-conduit", new [] { "pulse-conduit-top-0" }},
			{"armored-conveyor", new [] { "armored-conveyor-0-0" }},
			{"conduit", new [] { "conduit-top-0" }},
			{"thorium-reactor", new [] { "thorium-reactor" }},
			{"mass-driver",  new [] { "mass-driver-base", "mass-driver" }},
			{"cryofluidmixer", new [] { "cryofluidmixer-bottom", "cryofluidmixer-top" }},
			{"phase-weaver", new [] { "phase-weaver", "phase-weaver-weave" }},
			{"mechanical-drill", new [] { "mechanical-drill", "mechanical-drill-rotator", "mechanical-drill-top" }},
			{"pneumatic-drill", new [] { "pneumatic-drill", "pneumatic-drill-rotator", "pneumatic-drill-top" }},
			{"laser-drill", new [] { "laser-drill", "laser-drill-rotator", "laser-drill-top" }},
			{"blast-drill", new [] { "blast-drill", "blast-drill-rotator", "blast-drill-top" }},
			{"water-extractor", new [] { "water-extractor", "water-extractor-rotator", "water-extractor-top" }},
			{"cultivator", new [] { "cultivator", "cultivator-top" }},
			{"oil-extractor", new [] { "oil-extractor", "oil-extractor-rotator", "oil-extractor-top" }},
			{"liquid-tank", new [] { "liquid-tank-bottom", "liquid-tank-top" }},
			{"liquid-router", new [] { "liquid-router-bottom", "liquid-router-top" }},
			{"draug-factory", new [] { "draug-factory", "draug-factory-top" }},
			{"spirit-factory", new [] { "spirit-factory", "spirit-factory-top" }},
			{"phantom-factory", new [] { "phantom-factory", "phantom-factory-top" }},
			{"wraith-factory", new [] { "wraith-factory", "wraith-factory-top" }},
			{"ghoul-factory", new [] { "ghoul-factory", "ghoul-factory-top" }},
			{"revenant-factory", new [] { "revenant-factory", "revenant-factory-top" }},
			{"dagger-factory", new [] { "dagger-factory", "dagger-factory-top" }},
			{"crawler-factory", new [] { "crawler-factory", "crawler-factory-top" }},
			{"titan-factory", new [] { "titan-factory", "titan-factory-top" }},
			{"fortress-factory", new [] { "fortress-factory", "fortress-factory-top" }},
			{"scrap-wall-huge", new [] { "scrap-wall-huge1" }},
			{"scrap-wall-large", new [] { "scrap-wall-large1" }},
			{"scrap-wall", new [] { "scrap-wall1" }},
			{"pulverizer", new [] { "pulverizer", "pulverizer-rotator" }},
		};

		private static Image<Rgba32> RenderImage(Schematic schematic, SpriteAtlas spriteAtlas)
		{
			var spriteSet = spriteAtlas.SpriteSets["sprites.png"];
			using var spriteMap = Image.Load(spriteSet.FilePath);
			var finalImage = new Image<Rgba32>(schematic.Width * PixelsPerTile, schematic.Height * PixelsPerTile);

			foreach (var tile in schematic.Tiles)
			{
				RenderTileToImage(ref finalImage, tile, spriteSet, spriteMap);
			}

			return finalImage;
		}

		public static void SaveToFile(Schematic schematic, SpriteAtlas spriteAtlas, string destinationPath)
		{
			using var image = RenderImage(schematic, spriteAtlas);

			using var destinationFileStream = File.Create(destinationPath);
			image.SaveAsPng(destinationFileStream);
		}

		public static Stream RenderToStream(Schematic schematic, SpriteAtlas spriteAtlas)
		{
			using var image = RenderImage(schematic, spriteAtlas);
			var imageStream = new MemoryStream();

			image.SaveAsPng(imageStream);
			imageStream.Seek(0, SeekOrigin.Begin);

			return imageStream;
		}

		private static RotateMode RotateModeFromTileRotation(Tile tile)
		{
			switch (tile.Rotation)
			{
				case 0: // Right (default)
					return RotateMode.None;
				case 1: // Up
					return RotateMode.Rotate270;
				case 2: // Left
					return RotateMode.Rotate180;
				case 3: // Down
					return RotateMode.Rotate90;
			}

			return RotateMode.None;
		}

		private static void RenderTileToImage(ref Image<Rgba32> finalImage, Tile tile, SpriteSet spriteSet, Image spriteMap)
		{
			var renderModifier = BlockRenderModifiers.ContainsKey(tile.BlockName) ? BlockRenderModifiers[tile.BlockName] : null;

			if (renderModifier == null)
			{
				RenderSpriteToImage(ref finalImage, tile, spriteSet, spriteMap, tile.BlockName);
				return;
			}

			foreach (var spriteName in renderModifier)
			{
				RenderSpriteToImage(
					ref finalImage,
					tile,
					spriteSet,
					spriteMap,
					spriteName);
			}
		}

		private static void RenderSpriteToImage(
			ref Image<Rgba32> finalImage,
			Tile tile,
			SpriteSet spriteSet,
			Image spriteMap,
			string spriteName)
		{
			if (!spriteSet.Sprites.ContainsKey(spriteName))
				return;

			var sprite = spriteSet.Sprites[spriteName];
			if (sprite == null)
				return;

			var rotateMode = RotateModeFromTileRotation(tile);

			var spriteImage = spriteMap
				.Clone(ctx =>
				{
					ctx.Crop(new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height));
					ctx.Rotate(rotateMode);
				});

			var renderX = tile.X;
			var renderY = tile.Y;

			// For 2 block wide blocks at 0, 0 the schematic actually says 0, 1
			if (sprite.Width >= PixelsPerTile * 2)
			{
				renderY -= 1;
			}

			// For 3 block wide blocks at 0, 0 the schematic actually says 1, 1
			if (sprite.Width >= PixelsPerTile * 3)
			{
				renderX -= 1;
			}

			// For 4 block wide blocks at 0, 0 the schematic actually says 1, 2
			if (sprite.Width >= PixelsPerTile * 4)
			{
				renderY -= 1;
			}

			var spritePoint = new Point(renderX * PixelsPerTile, renderY * PixelsPerTile);

			finalImage = finalImage
				.Clone(ctx => ctx.DrawImage(spriteImage, spritePoint, 1));

			spriteImage.Dispose();
		}

		public static void RenderSprite(Sprite sprite, string destinationPath)
		{
			using var fromSprite = Image
				.Load(sprite.SpriteSet.FilePath)
				.Clone(ctx => ctx.Crop(new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height)));

			using var destinationFileStream = File.Create(destinationPath);
			fromSprite.SaveAsPng(destinationFileStream);
		}
	}
}