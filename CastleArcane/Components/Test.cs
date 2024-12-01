using CastleArcane.Entities;
using CastleArcane.Model;
using SadConsole.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleArcane.Components
{
	internal class Stepper : SingleHostComponent<RigidEntity>
	{
		int _tick = 0;
		Direction.Types _prev;

		public int PrimaryInterval { get; set; } = 1;
		public int SecondaryInterval { get; set; } = 0;
		public int Threshold { get; set; } = int.MaxValue;

		public Direction Direction { get; set; }

		public int Accumulator { get; private set; }

		public override bool IsUpdate => true;

		
		public override void Update(IScreenObject host, TimeSpan delta)
		{
			if (--_tick <= 0)
			{
				if (Direction == Direction.None)
				{
					_tick = 0;
					Accumulator = 0;
				}
				else
				{
					_tick = Accumulator < Threshold ? PrimaryInterval : SecondaryInterval;
					if (SingleHost!.TryMove(Direction))
					{
						_prev = Direction;
						Accumulator++;
					}
				}
			}
		}

		


	}
}
