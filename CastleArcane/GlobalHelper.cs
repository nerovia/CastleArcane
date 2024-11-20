using CastleArcane.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleArcane
{
	[Obsolete("This is for development, do not forget to get rid of this someday.")]
	internal static class GlobalHelper
	{
		public static SpriteMap? CurrentSpriteMap { get; set; }
	}
}
