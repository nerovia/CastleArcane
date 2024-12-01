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
		readonly Stepper _stepX;
		readonly Stepper _stepY;
		readonly InputComponent _input;

		public PlayerEntity(AnimatedScreenObject appearance) : base(appearance)
		{
			_input = new InputComponent(GameSettings.KeyBindings);
			_stepX = new Stepper() { PrimaryInterval = 8, SecondaryInterval = 4, Threshold = 3 };
			//_stepY= new Stepper() { PrimaryInterval = 8, SecondaryInterval = 4, Threshold = 3 };

			SadComponents.Add(_input);
			SadComponents.Add(_stepX);
			//SadComponents.Add(_stepY);
		}

		public override void Update(TimeSpan delta)
		{
			base.Update(delta);

			_stepX.Direction = Direction.GetCardinalDirection(_input.InputState.Direction);

			//_stepX.Direction = _input.InputState.Direction.X switch
			//{
			//	> 0 => Direction.Right,
			//	0 => Direction.None,
			//	< 0 => Direction.Left
			//};

			//_stepY.Direction = _input.InputState.Direction.Y switch
			//{
			//	> 0 => Direction.Up,
			//	0 => Direction.None,
			//	< 0 => Direction.Down
			//};

		}
	}
}
