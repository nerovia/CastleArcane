
using SadConsole.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CastleArcane.Utility
{
	public static class Extensions
	{
		public static Point ToSadPoint(this Microsoft.Xna.Framework.Vector2 point)
		{
			return new((int)MathF.Round(point.X), (int)MathF.Round(point.Y));
	
		}

		public static Point Normalize(this Point point) => new(int.Sign(point.X), int.Sign(point.Y));

		public static IEnumerable<T> OfComponent<T>(this IEnumerable<IScreenObject> screenObjects) where T : class, IComponent
		{
			return from obj in screenObjects
				   let component = obj.GetSadComponent<T>()
				   where component != null
				   select component;
		}
	}
}
