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

		private static readonly Dictionary<string, BlockRenderModifier> BlockRenderModifiers = new Dictionary<string, BlockRenderModifier>
		{
			{"conveyor", new BlockRenderModifier
			{
				AlternativeSprites = new [] { "conveyor-0-0" }
			}},
			{"titanium-conveyor", new BlockRenderModifier
			{
				AlternativeSprites = new [] { "titanium-conveyor-0-0" }
			}},
			{"pulse-conduit", new BlockRenderModifier
			{
				AlternativeSprites = new [] { "pulse-conduit-top-0" }
			}},
			{"armored-conveyor", new BlockRenderModifier
			{
				AlternativeSprites = new [] { "armored-conveyor-0-0" }
			}},
			{"conduit", new BlockRenderModifier
			{
				AlternativeSprites = new [] { "conduit-top-0" }
			}},
			{"thorium-reactor", new BlockRenderModifier
			{
				XPositionModifier = -1,
				YPositionModifier = -1,
				AlternativeSprites = new [] { "thorium-reactor" }
			}},
			{"mass-driver", new BlockRenderModifier
			{
				XPositionModifier = -1,
				YPositionModifier = -1,
				AlternativeSprites = new [] { "mass-driver-base", "mass-driver" }
			}},
			{"cryofluidmixer", new BlockRenderModifier
			{
				YPositionModifier = -1,
				AlternativeSprites = new [] { "cryofluidmixer-bottom", "cryofluidmixer-top" }
			}},
			{"phase-weaver", new BlockRenderModifier
			{
				YPositionModifier = -1,
				AlternativeSprites = new [] { "phase-weaver", "phase-weaver-weave" }
			}}
		};

		public static void Visualize(Schematic schematic, SpriteAtlas spriteAtlas, string destinationPath)
		{
			var spriteSet = spriteAtlas.SpriteSets["sprites.png"];
			using var spriteMap = Image.Load(spriteSet.FilePath);
			var finalImage = new Image<Rgba32>(schematic.Width * PixelsPerTile, schematic.Height * PixelsPerTile);

			foreach (var tile in schematic.Tiles)
			{
				RenderTileToImage(ref finalImage, tile, spriteSet, spriteMap);
			}

			using var destinationFileStream = File.Create(destinationPath);
			finalImage.SaveAsPng(destinationFileStream);

			finalImage.Dispose();
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
			var rotateMode = RotateModeFromTileRotation(tile);

			if (renderModifier == null)
			{
				RenderSpriteToImage(ref finalImage, rotateMode, tile.X, tile.Y, spriteSet, spriteMap, tile.BlockName, true);
				return;
			}

			if (renderModifier.AlternativeSprites == null)
			{
				RenderSpriteToImage(
					ref finalImage,
					rotateMode,
					tile.X + renderModifier.XPositionModifier,
					tile.Y + renderModifier.YPositionModifier,
					spriteSet,
					spriteMap,
					tile.BlockName,
					false);
				return;
			}

			// Render multiple sprites
			foreach (var spriteName in renderModifier.AlternativeSprites)
			{
				RenderSpriteToImage(
					ref finalImage,
					rotateMode,
					tile.X + renderModifier.XPositionModifier,
					tile.Y + renderModifier.YPositionModifier,
					spriteSet,
					spriteMap,
					spriteName,
					false);
			}
		}

		private static void RenderSpriteToImage(
			ref Image<Rgba32> finalImage,
			RotateMode rotateMode,
			int x,
			int y,
			SpriteSet spriteSet,
			Image spriteMap,
			string spriteName,
			bool applyPositionFix)
		{
			if (!spriteSet.Sprites.ContainsKey(spriteName))
				return;

			var sprite = spriteSet.Sprites[spriteName];
			if (sprite == null)
				return;

			var spriteImage = spriteMap
				.Clone(ctx =>
				{
					ctx.Crop(new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height));
					ctx.Rotate(rotateMode);
				});

			// For 2 block wide blocks at 0, 0 the schematic actually says 1, 1
			if (applyPositionFix && sprite.Width >= PixelsPerTile * 2)
			{
				y -= 1;
			}

			// For 3 block wide blocks at 0, 0 the schematic actually says 1, 2
			if (applyPositionFix && sprite.Width >= PixelsPerTile * 3)
			{
				x -= 1;
			}

			// For 3 block wide blocks at 0, 0 the schematic actually says 1, 2
			if (applyPositionFix && sprite.Width >= PixelsPerTile * 4)
			{
				y -= 1;
			}

			var spritePoint = new Point(x * PixelsPerTile, y * PixelsPerTile);

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

	internal class BlockRenderModifier
	{
		public int XPositionModifier { get; set; }

		public int YPositionModifier { get; set; }

		public string[] AlternativeSprites { get; set; }
	}
}