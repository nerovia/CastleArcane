
using SadConsole.Components;

namespace CastleArcane.Components;

/// <summary>
/// A component that is strongly linked to an entity.
/// </summary>
public class SingleHostComponent<THost> : ComponentBase where THost : class, IScreenObject
{
	public THost? SingleHost { get; private set; }

	public override void OnAdded(IScreenObject host)
	{
		if (SingleHost is not null)
			throw new ArgumentException("Component is already assigned to other host");
		if (host is not THost singleHost)
			throw new ArgumentException($"Host must be of type {typeof(THost)}");
		SingleHost = singleHost;
	}

	public override void OnRemoved(IScreenObject host)
	{
		SingleHost = null;
	}

	protected T RequireHostComponent<T>() where T : class, IComponent
	{
		return SingleHost!.GetSadComponent<T>()
			?? throw new ArgumentException($"Component requires host component {typeof(T)}");
	}
}
