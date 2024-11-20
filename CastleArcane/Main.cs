using CastleArcane.Localization;
using CastleArcane.Screens;
using SadConsole.Configuration;

Settings.WindowTitle = Strings.GameTitle;

Builder gameStartup = new Builder()
	.SetScreenSize(GameSettings.GameWidth, GameSettings.GameHeight)
	.SetStartingScreen<TestScreen>()
	.IsStartingScreenFocused(true)
	.ConfigureFonts("Content/CHUNKY.json")
	;


Game.Create(gameStartup);
Game.Instance.DefaultFontSize = IFont.Sizes.Four;
Game.Instance.Keyboard.RepeatDelay = float.PositiveInfinity;
Game.Instance.Keyboard.InitialRepeatDelay = float.PositiveInfinity;
Game.Instance.Run();
Game.Instance.Dispose();