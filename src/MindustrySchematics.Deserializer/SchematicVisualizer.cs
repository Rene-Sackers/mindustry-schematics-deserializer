using System.IO;
using System.Threading.Tasks;
using MindustrySchematics.Deserializer.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace MindustrySchematics.Deserializer
{
	public class SchematicVisualizer
	{
		public async Task Visualize(Schematic schematic)
		{

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
