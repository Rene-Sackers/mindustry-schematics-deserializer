using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using MindustrySchematics.Deserializer.Extensions;

namespace MindustrySchematics.Deserializer.Models
{
	public class SpriteAtlas
	{
		protected readonly Dictionary<string, SpriteSet> SpriteSetsInternal = new Dictionary<string, SpriteSet>();

		public string FilePath { get; protected set; }

		public IReadOnlyDictionary<string, SpriteSet> SpriteSets => SpriteSetsInternal;

		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public static SpriteAtlas FromFile(string path)
		{
			using var fileStream = File.OpenRead(path);
			using var reader = new StreamReader(fileStream);

			var spriteAtlas = new SpriteAtlas
			{
				FilePath = path
			};

			SpriteSet currentSpriteSet = null;

			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine();
				if (string.IsNullOrWhiteSpace(line))
				{
					currentSpriteSet = ReadNewSpriteSet(reader, spriteAtlas);

					spriteAtlas.SpriteSetsInternal.Add(currentSpriteSet.FileName, currentSpriteSet);

					continue;
				}

				if (line.StartsWith("  "))
					continue;

				var sprite = ReadSprite(line, reader, currentSpriteSet);

				currentSpriteSet.Sprites.Add(sprite.Name, sprite);
			}

			return spriteAtlas;
		}

		private static Sprite ReadSprite(string line, TextReader reader, SpriteSet spriteSet)
		{
			var spriteName = line;
			var (x, y) = ParseLineValues(reader.ReadUntilLineStartsWith("  xy"), "xy");
			var (width, height) = ParseLineValues(reader.ReadUntilLineStartsWith("  size"), "size");

			return new Sprite
			{
				SpriteSet = spriteSet,
				Name = spriteName,
				Width = width,
				Height = height,
				X = x,
				Y = y
			};
		}

		private static SpriteSet ReadNewSpriteSet(StreamReader reader, SpriteAtlas spriteAtlas)
		{
			var newSpriteSet = new SpriteSet
			{
				SpriteAtlas = spriteAtlas,
				FileName = reader.ReadLine()
			};

			var imagePath = Path.Combine(Path.GetDirectoryName(newSpriteSet.SpriteAtlas.FilePath), newSpriteSet.FileName);

			newSpriteSet.FilePath = imagePath;

			// ReSharper disable once PossibleNullReferenceException
			var size = reader.ReadLine().Replace("size: ", null).Split(',');
			newSpriteSet.Width = int.Parse(size[0]);
			newSpriteSet.Height = int.Parse(size[1]);

			reader.ReadLine(); // Format
			reader.ReadLine(); // Filter
			reader.ReadLine(); // Repeat

			return newSpriteSet;
		}

		private static Tuple<int, int> ParseLineValues(string line, string parameter)
		{
			var xy = line.Replace($"  {parameter}: ", null).Split(',');
			return new Tuple<int, int>(int.Parse(xy[0]), int.Parse(xy[1]));
		}
	}

	public class SpriteSet
	{
		public SpriteAtlas SpriteAtlas { get; set; }

		public string FileName { get; set; }

		public string FilePath { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public Dictionary<string, Sprite> Sprites { get; set; } = new Dictionary<string, Sprite>();
	}

	public class Sprite
	{
		public SpriteSet SpriteSet { get; set; }

		public string Name { get; set; }

		public int X { get; set; }

		public int Y { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }
	}
}
