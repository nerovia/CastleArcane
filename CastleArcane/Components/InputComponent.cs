using CastleArcane.Model;
using SadConsole.Components;
using SadConsole.Input;

namespace CastleArcane.Components;

record KeyBindings
{
	public Keys LeftKey { get; set; }
	public Keys RightKey { get; set; }
	public Keys JumpKey { get; set; }
	public Keys AttackKey { get; set; }
	public Keys SpecialKey { get; set; }
}

internal class InputComponent(KeyBindings keys) : UpdateComponent, IInputProvider
{
	private InputState state;

	public InputState InputState { get => state; }
	
	public event Action? Attacked;

	public event Action? Jumped;

	public override void OnAdded(IScreenObject host)
	{
		base.OnAdded(host);
	}

	public override void Update(IScreenObject host, TimeSpan delta)
	{
		state.Direction = Point.Zero;
		HandleStick(keys.LeftKey, ref state.Direction, new(-1, 0));
		HandleStick(keys.RightKey, ref state.Direction, new(1, 0));
		HandleStick(keys.JumpKey, ref state.Direction, new(0, -1));
		HandleStick(Keys.Down, ref state.Direction, new(0, 1));
		HandleTrigger(keys.AttackKey, ref state.Attack, Attacked);
		HandleTrigger(keys.JumpKey, Jumped);
	}

	static void HandleStick(Keys key, ref Point state, Point vec)
	{
		if (SadConsole.Game.Instance.Keyboard.IsKeyDown(key))
			state += vec;
	}

	static void HandleTrigger(Keys key, ref bool triggerState, Action? triggerAction)
	{
		HandleTrigger(key, triggerAction);
		triggerState = SadConsole.Game.Instance.Keyboard.IsKeyDown(key);
	}

	static void HandleTrigger(Keys key, Action? triggerAction)
	{
		if (SadConsole.Game.Instance.Keyboard.IsKeyPressed(key))
			triggerAction?.Invoke();
	}
}