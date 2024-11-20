using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CastleArcane.Model;

// v -> s  n
// v -> c  y
// v -> 



[System.Flags]
public enum TileModifier
{
	None       = 0x00, // The tile has no attributes
	BlockAll   = 0x0f, // Block all movement
	BlockLeft  = 0x01, // Block left movement
	BlockRight = 0x02, // Block right movement
	BlockUp    = 0x04, // Block up movement
	BlockDown  = 0x08, // Block down movement
	HaltFall   = 0x10, // Halt fall movement
	NoFall = 0x20,
	//Void       = 0x10,  // Allows jumping through that tile
	//Climbable  = 0x20, // Allows climbing on that tile
}


public record Tile
{
	public TileModifier Modifier { get; init; }
	public ColoredGlyphBase Appearance { get; init; }

	public readonly static Tile Solid = new()
	{
		Modifier = TileModifier.BlockAll,
		Appearance = new ColoredGlyph(Color.Gray, Color.Transparent, '#')
	};

	public readonly static Tile Void = new()
	{
		Modifier = TileModifier.None,
		Appearance = new ColoredGlyph(Color.Transparent, Color.Transparent, ' ')
	};

	public readonly static Tile Ladder = new()
	{
		Modifier = TileModifier.HaltFall | TileModifier.NoFall,
		Appearance = new ColoredGlyph(Color.Brown, Color.Transparent, '=')
	};

	public readonly static Tile Semisolid = new()
	{
		Modifier = TileModifier.HaltFall,
		Appearance = new ColoredGlyph(Color.Blue, Color.Transparent, '%')
	};

	public bool IsGround => Modifier.HasFlag(TileModifier.BlockDown) || Modifier.HasFlag(TileModifier.HaltFall);

	public bool IsBlockedFor(Direction.Types direction)
	{
		return Modifier.HasFlag(direction switch
		{
			Direction.Types.Up => TileModifier.BlockUp,
			Direction.Types.Down => TileModifier.BlockDown,
			Direction.Types.Left => TileModifier.BlockLeft,
			Direction.Types.Right => TileModifier.BlockRight,
			_ => throw new ArgumentException("Cardinal direction required", nameof(direction))
		});
	}
}

