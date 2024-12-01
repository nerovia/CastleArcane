using CastleArcane.Entities;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleArcane.Components
{
	internal class EnemyComponent : SingleHostComponent<RigidEntity>
	{
		Stepper _movement;

		public override bool IsUpdate => true;

		public override void Update(IScreenObject host, TimeSpan delta)
		{

			var test = new GoRogue.Pathing.GoalMap()
		}

		public override void OnAdded(IScreenObject host)
		{
			
			_movement = host.GetSadComponent<Stepper>() ?? throw new ArgumentException();
		}

	}
}
