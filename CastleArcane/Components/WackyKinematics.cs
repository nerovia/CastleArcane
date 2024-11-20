using CastleArcane.Entities;
using CastleArcane.Model;
using CastleArcane.Utility;
using SadConsole.Components;
using SadConsole.EasingFunctions;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace CastleArcane.Components
{


    interface IKinematicsComponent : IComponent, IDiscreteKinematics;



	/// <summary>
	/// This component models the kinematic behavior for rigid entities.
	/// The velocity is based on frames, where a speed of 1 results in a step per frame, a speed of 0.5 in a step every other frame, etc.
	/// The entities movement is restricted to at most 1 step per frame, even though the velocity may exceed that value.
	/// Acceleration behaves differently from real world physics.
	/// (I mean the maths is kinda wacky and the physics all off, but it feels alright to play)
	/// </summary>
	internal class WackyKinematics : SingleHostComponent<RigidEntity>, IKinematicsComponent
    {
		int _ticksX;
		int _ticksY;
		float _maxSpeedX;
		float _maxSpeedY;
		public int StepCountX { get; private set; }
		public int StepCountY { get; private set; }

		public event SteppingEventHandler? Stepping;
		public event SteppedEventHandler? Stepped;

		public override bool IsUpdate => true;

		/// <summary>
		/// 
		/// </summary>
		public float VelocityX { get; set; }
		public float VelocityY { get; set; }
		
		public float AccelerationX { get; set; }
		public float AccelerationY { get; set; }

		/// <summary>
		/// The maximum speed at which to step. This value must
		/// </summary>
		public float MaxSpeedX
		{
			get => _maxSpeedX;
			set
			{
				if (value < 0f || value > 1f)
					throw new ArgumentOutOfRangeException(nameof(value));
				_maxSpeedX = value;
			}
		}

		public float MaxSpeedY
		{
			get => _maxSpeedY;
			set
			{
				if (value < 0f || value > 1f)
					throw new ArgumentOutOfRangeException(nameof(value));
				_maxSpeedY = value;
			}
		}

		public override void Update(IScreenObject host, TimeSpan delta)
		{
			if (--_ticksX <= 0)
			{
				VelocityX += AccelerationX;
				var speedX = float.Abs(VelocityX);
				if (speedX > 0)
				{
					if (TryMove(VelocityX < 0 ? Direction.Types.Left : Direction.Types.Right, ++StepCountX))
					{
						_ticksX = (int)(1f / float.Min(speedX, MaxSpeedX));
					}
					else
					{
						VelocityX = 0;
						StepCountX = 0;
					}
				}
				else
				{
					StepCountX = 0;
				}
			}

			if (--_ticksY <= 0)
			{
				VelocityY += AccelerationY;
				float speedY = float.Abs(VelocityY);
				if (speedY > 0)
				{
					if (TryMove(VelocityY < 0 ? Direction.Types.Down : Direction.Types.Up, ++StepCountY))
					{
						_ticksY = (int)(1f / float.Min(speedY, MaxSpeedX));
					}
					else
					{
						VelocityY = 0;
						StepCountY = 0;
					}
				}
				else
				{
					StepCountY = 0;
				}
			}
		}

		public bool TryMove(Direction.Types direction, int counter)
		{
			var step = new StepInfo(direction, counter);
			var stepping = new SteppingEventArgs(step);
			if (!SingleHost!.Scene.CanMove(SingleHost, direction, out Point newPosition))
				return false;
			OnStepping(stepping);
			if (stepping.WasCanceled)
				return false;
			SingleHost!.Position = newPosition;
			OnStepped(new SteppedEventArgs(step));
			return true;
		}

		protected virtual void OnStepping(SteppingEventArgs args)
		{
			Stepping?.Invoke(this, args);
		}

		protected virtual void OnStepped(SteppedEventArgs args)
		{
			Stepped?.Invoke(this, args);
		}

	}
}
