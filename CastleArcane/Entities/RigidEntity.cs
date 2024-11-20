using CastleArcane.Components;
using CastleArcane.Model;
using CommunityToolkit.HighPerformance;
using SadConsole.EasingFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CastleArcane.Entities
{
	public class RigidEntity : SceneEntity
	{
		public Point Anchor { get; set; }

		public Point AnchorPosition => Position + Anchor;

		public Rectangle Bounds { get; set; }

		public RigidEntity(AnimatedScreenObject appearance) : base(appearance)
		{
			Bounds = AppearanceSurface!.DefaultCollisionRectangle;
			Anchor = Bounds.Center.WithY(Bounds.MaxExtentY);
		}

		public RigidEntity(ColoredGlyph appearance) : base(appearance)
		{
			Anchor = Point.Zero;
			Bounds = new Rectangle(0, 0, 1, 1);
		}
	}
}
