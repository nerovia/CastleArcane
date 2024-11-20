using CastleArcane.Model;
using SadConsole.Entities;

namespace CastleArcane.Entities
{
	public delegate void LifetimeEventHandler(SceneEntity sender, IScene scene);

	public class LifetimeException(string message) : Exception(message);

	public class SceneEntity : Entity
	{
		public IScene? Scene { get; private set; }

		public SceneEntity(AnimatedScreenObject appearance) : base(appearance, 0) { }

		public SceneEntity(ColoredGlyphBase appearance) : base(appearance, 0) { }

		public event LifetimeEventHandler? Spawned;

		public event LifetimeEventHandler? Despawned;

		protected virtual void OnSpawned(IScene scene)
		{
			Spawned?.Invoke(this, scene);
		}

		protected virtual void OnDespawned(IScene scene)
		{
			Despawned?.Invoke(this, scene);
		}

		internal void Spawn(IScene scene)
		{
			if (Scene != null)
				throw new LifetimeException("The entity is already present in a scene");
			Scene = scene;
			OnSpawned(scene);
		}

		internal void Despawn(IScene scene)
		{
			if (Scene != scene)
				throw new LifetimeException("The entity is not present in any scene");
			OnDespawned(scene);
			Scene = null;
		}
	}
}
