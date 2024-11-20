using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleArcane.Components;

/// <summary>
/// Detects collisions between entities
/// </summary>
public class CollisionComponent : SingleHostComponent<Entity>
{	
	public uint CollisionMask { get; set; }

	public uint CollisionLayer { get; set; }

	public Rectangle Bounds { get; set; }
	
	public Rectangle AbsoluteBounds { get => Bounds.Translate(SingleHost!.Position); }

	public event CollisionHandler? Collided;

	public static void Check(CollisionComponent a, CollisionComponent b)
	{
		if ((a.CollisionLayer & b.CollisionMask) == 0)
			return;

		if ((b.CollisionLayer & a.CollisionMask) == 0)
			return;

		var intersect = Rectangle.GetIntersection(a.AbsoluteBounds, b.AbsoluteBounds);

		if (intersect.IsEmpty)
			return;
		a.OnCollision(b.SingleHost!, intersect);
		b.OnCollision(a.SingleHost!, intersect);
	}

	public static void CheckAll(CollisionComponent[] colliders)
	{
		for (int i = 0; i < colliders.Length - 1; i++)
		{
			var a = colliders[i];
			for (int j = i + 1; j < colliders.Length; j++)
			{
				var b = colliders[j];
				CollisionComponent.Check(a, b);
			}
		}
	}

	protected virtual void OnCollision(Entity other, Rectangle intersect)
	{
		Collided?.Invoke(new(SingleHost!, other, intersect));
	}
}

public record Collision(Entity Self, Entity Other, Rectangle Intersect);

public delegate void CollisionHandler(Collision details);
