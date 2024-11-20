using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleArcane.Model
{
	interface ISpriteMap
	{
		ColoredGlyphBase GetGlyph(string key);
		ICellSurface GetSurface(string key);
		AnimatedScreenObject GetAnimated(string key, string? name = null);
	}

	interface ISprite;

	record AnimatedSprite(Rectangle[] Frames) : ISprite;

	record GlyphSprite(Point Position) : ISprite;

	record SurfaceSprite(Rectangle Bounds) : ISprite;
	
	class SpriteMap : ISpriteMap
	{
		public ICellSurface Source { get; }

		public Dictionary<string, ISprite> Sprites { get; } = new();

		public SpriteMap(ICellSurface source)
		{
			Source = source;
		}

		public AnimatedScreenObject GetAnimated(string key, string? name = null)
		{ 
			switch (Sprites[key])
			{
				case AnimatedSprite animated:
					var frames = from bounds in animated.Frames select Source.GetSubSurface(bounds);
					return new AnimatedScreenObject(name ?? key, frames);

				case SurfaceSprite surface:
					frames = [ Source.GetSubSurface(surface.Bounds) ];
					return new AnimatedScreenObject(name ?? key, frames);

				default:
					throw new ArgumentException("Sprite cannot be animated");
			}
		}

		public ColoredGlyphBase GetGlyph(string key)
		{
			if (Sprites[key] is not GlyphSprite info)
				throw new ArgumentException("Sprite is not a glyph");
			return Source[info.Position];
		}

		public ICellSurface GetSurface(string key)
		{
			if (Sprites[key] is not SurfaceSprite info)
				throw new ArgumentException("Sprite is not surface");
			return Source.GetSubSurface(info.Bounds);
		}
	}


}
