using CastleArcane.Components;
using CastleArcane.Model;
using SadConsole.Input;

internal static class GameSettings
{
	public const int GameWidth = 32;
	public const int GameHeight = 32;
	public static KeyBindings KeyBindings { get; } = new()
	{
		LeftKey = Keys.Left,
		RightKey = Keys.Right,
		JumpKey = Keys.Up,
		AttackKey = Keys.Space
	};
}
