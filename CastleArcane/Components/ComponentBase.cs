using SadConsole.Components;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace CastleArcane.Components;

/// <summary>
/// A component that holds data about its host.
/// </summary>
public abstract class ComponentBase : IComponent
{
	public uint SortOrder { get; set; }

	public virtual bool IsUpdate => false;

	public virtual bool IsRender => false;

	public virtual bool IsMouse => false;

	public virtual bool IsKeyboard => false;

	public abstract void OnAdded(IScreenObject host);

	public abstract void OnRemoved(IScreenObject host);

	public virtual void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
	{
		throw new NotImplementedException();
	}

	public virtual void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
	{
		throw new NotImplementedException();
	}

	public virtual void Render(IScreenObject host, TimeSpan delta)
	{
		throw new NotImplementedException();
	}

	public virtual void Update(IScreenObject host, TimeSpan delta)
	{
		throw new NotImplementedException();
	}
}
