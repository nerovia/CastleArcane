
namespace CastleArcane.Model;

interface IInputProvider
{
	public event Action? Attacked;

	public event Action? Jumped;

	InputState InputState { get; }
}

public struct InputState
{
	public bool Attack;

	public Point Direction;
}
