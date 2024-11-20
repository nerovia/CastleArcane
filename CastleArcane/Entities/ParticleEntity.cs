using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleArcane.Entities
{
	internal class ParticleEntity : SceneEntity
	{
		public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.5);

		public ParticleEntity(ColoredGlyphBase appearance) : base(appearance)
		{
		}

		public override void Update(TimeSpan delta)
		{
			base.Update(delta);
			Duration -= delta;
			if (Duration <= TimeSpan.Zero)
				Scene.Despawn(this);
		}
	}
}
