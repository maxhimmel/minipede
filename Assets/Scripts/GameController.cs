using Minipede.Gameplay.LevelPieces;
using Zenject;

namespace Minipede.Gameplay
{
	public class GameController : IInitializable
	{
		private readonly PlayerSpawner _playerSpawner;
		private readonly LevelGraph _levelGraph;

		public GameController( PlayerSpawner playerSpawner,
			LevelGraph levelGraph )
		{
			_playerSpawner = playerSpawner;
			_levelGraph = levelGraph;
		}

		public async void Initialize()
		{
			await _levelGraph.GenerateLevel();
			_playerSpawner.Spawn();
		}
	}
}
