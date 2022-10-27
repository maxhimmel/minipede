using Minipede.Gameplay.Player;
using Minipede.Installers;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class PlayerSpawner
	{
		private readonly PlayerController.Factory _factory;
		private readonly IOrientation _spawnPoint;

		public PlayerSpawner( 
			DiContainer container,
			PlayerController.Factory factory,
			GameplaySettings.Player settings )
		{
			_factory = factory;
			_spawnPoint = container.ResolveId<IOrientation>( settings.SpawnPointId );
		}

		public PlayerController Create()
		{
			var newPlayer = _factory.Create();
			newPlayer.transform.SetParent( _spawnPoint.Parent );
			newPlayer.transform.SetPositionAndRotation( _spawnPoint.Position, _spawnPoint.Rotation );

			return newPlayer;
		}
    }
}
