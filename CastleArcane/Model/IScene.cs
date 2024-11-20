using CastleArcane.Entities;
using SadConsole.Entities;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.SpatialMaps;

namespace CastleArcane.Model;

/// <summary>
/// Manages the environment etc.
/// 
/// </summary>
public interface IScene
{
	IEnumerable<Entity> Entities { get; }

	IGridView<Tile> Map { get; }

	void Spawn(Point position, Entity entity);

	void Despawn(Entity entity);

	bool CanMove(RigidEntity entity, Direction.Types offset, out Point newPosition);
}
