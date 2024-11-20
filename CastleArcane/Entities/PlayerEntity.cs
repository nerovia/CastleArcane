using CastleArcane.Components;
using CastleArcane.Model;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleArcane.Entities
{
    internal class PlayerEntity : RigidEntity
	{
		readonly WackyKinematics _kinematics;
		readonly IInputProvider _input;

		public PlayerEntity(AnimatedScreenObject appearance) : base(appearance)
		{
			_kinematics = new WackyKinematics() { MaxSpeedX = 0.5f, MaxSpeedY = 0.5f };
			_input = new InputComponent(GameSettings.KeyBindings);
			SadComponents.Add((InputComponent)_input);
			SadComponents.Add(_kinematics);
			_input.Jumped += () =>
			{
				_kinematics.VelocityY = 0.625f;
			};
			_kinematics.Stepping += (s, e) =>
			{
				Debug.WriteLine(e.Step.Counter);
				var host = (s as WackyKinematics).SingleHost!;

				void SpawnParticle(string key)
				{
					Scene.Spawn(host.AnchorPosition, new ParticleEntity(GlobalHelper.CurrentSpriteMap.GetGlyph(key)));
				}


				switch (e.Step.Direction)
				{
					case Direction.Types.Left:
						host.AppearanceSurface!.Animation.CurrentFrameIndex = 1;
						SpawnParticle(e.Step.Counter == 1 ? "gust-l" : "gust-h");
						break;

					case Direction.Types.Right:
						host.AppearanceSurface!.Animation.CurrentFrameIndex = 0;
						SpawnParticle(e.Step.Counter == 1 ? "gust-r" : "gust-h");
						break;

					case Direction.Types.Up:
						SpawnParticle(e.Step.Counter == 1 ? "gust-u" : "gust-v");
						break;

				}
			};
			_kinematics.AccelerateY(-0.125f, false);
		}

		public override void Update(TimeSpan delta)
		{
			base.Update(delta);

			_kinematics.VelocityX = _input.InputState.Direction.X * 0.125f;
			

		}
	}
}
