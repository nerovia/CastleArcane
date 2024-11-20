using CastleArcane.Components;
using CastleArcane.Entities;
using CastleArcane.Model;
using CastleArcane.Utility;
using SadConsole.Entities;
using SadRogue.Primitives.GridViews;
using System.Runtime.InteropServices.JavaScript;
using System.Text;

namespace CastleArcane.Screens
{
    internal class TestScreen : ScreenObject, IScene
    {
        private readonly ScreenSurface _mainSurface;
        private readonly EntityManager _entityManager;
        private readonly SpriteMap _spriteMap;

        public TestScreen()
        {
            _entityManager = new EntityManager();
            _spriteMap = new SpriteMap(Playscii.ReadFile("Content/sprites.psci").ToCellSurface())
            {
                Sprites =
                {
                    { "player", new AnimatedSprite([ new(0, 0, 1, 2), new(1, 0, 1, 2) ]) },
                    { "gust-r", new GlyphSprite(new(0, 2)) },
					{ "gust-l", new GlyphSprite(new(1, 2)) },
                    { "gust-u", new GlyphSprite(new(2, 2)) },
					{ "gust-h", new GlyphSprite(new(2, 0)) },
					{ "gust-v", new GlyphSprite(new(2, 1)) }
				}
            };

            GlobalHelper.CurrentSpriteMap = _spriteMap;

            var player = CreatePlayer();
			Spawn(new(12, 8), player);
            _mainSurface = new ScreenSurface(GameSettings.GameWidth, GameSettings.GameHeight);
            _mainSurface.SadComponents.Add(_entityManager);

         

            Map = new LambdaGridView<Tile>(GameSettings.GameWidth, GameSettings.GameHeight, pos =>
            {
                return Array.Find([Tile.Solid, Tile.Void, Tile.Semisolid, Tile.Ladder],
                    it => _mainSurface.GetCellAppearance(pos.X, pos.Y).Glyph == it.Appearance.Glyph)
                    ?? Tile.Void;
            });

            _mainSurface.DrawLine((10, 10), (20, 10), '#', Color.Gray);
			_mainSurface.DrawLine((20, 5), (20, 8), '#', Color.Gray);
            _mainSurface.DrawLine((4, GameSettings.GameHeight - 1), (4, 10), '=', Color.Blue);
            _mainSurface.DrawLine((10, GameSettings.GameHeight - 5), (20, GameSettings.GameHeight - 5), '%', Color.Pink);
            _mainSurface.DrawBox(_mainSurface.Surface.Area, ShapeParameters.CreateBorder(new ColoredGlyph(Color.Gray, Color.Black, '#')));

			Children.Add(_mainSurface);
        }


        Entity CreatePlayer()
        {
            return new PlayerEntity(_spriteMap.GetAnimated("player"))
            {
                ZIndex = 10,
            };
        }


        public IEnumerable<Entity> Entities => _entityManager.Entities;

		public IGridView<Tile> Map { get; }

		public void Despawn(Entity entity)
		{
            _entityManager.Remove(entity);
			if (entity is SceneEntity sceneEntity)
				sceneEntity.Despawn(this);
		}

		public void Spawn(Point position, Entity entity)
		{
            if (_entityManager.Contains(entity))
                throw new ArgumentException("Entity already present");
            _entityManager.Add(entity);
            entity.Position = position;
            if (entity is SceneEntity sceneEntity)
                sceneEntity.Spawn(this);
        }

        public bool CanMove(RigidEntity entity, Direction.Types offset, out Point position)
        {
            position = entity.Position;
			var newPosition = entity.Position + offset;

			var rect = entity.Bounds;
			var boundary = rect.Translate(newPosition).PositionsOnSide(offset);
            if (boundary.Any(it => Map[it].IsBlockedFor(offset)))
                return false;
           
            position = newPosition;

            return true;
		}
	}
}
