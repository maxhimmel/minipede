using Minipede.Gameplay.LevelPieces;
using Zenject;

namespace Minipede.Gameplay
{
	public class GameController : IInitializable
	{
		private readonly PlayerSpawner _playerSpawner;

		public GameController( PlayerSpawner playerSpawner )
		{
			_playerSpawner = playerSpawner;
		}

		public void Initialize()
		{
			_playerSpawner.Spawn();
		}
	}
}
