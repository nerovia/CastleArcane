using CastleArcane.Model;
using SadConsole.Components;
using SadConsole.Entities;

namespace CastleArcane.Components;

/// <summary>
/// Moves the host with the target entity
/// </summary>
class FollowComponent : UpdateComponent
{
	public Entity? Target { get; set; }

	public Point Offset { get; set; }

	public override void Update(IScreenObject host, TimeSpan delta)
	{
		if (Target == null)
			return;
		host.Position = Target.Position.Translate(Offset);
	}
}