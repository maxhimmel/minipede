using Minipede.Gameplay.Player;
using Minipede.Installers;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class ShipSpawner
	{
		private readonly Ship.Factory _factory;
		private readonly IOrientation _spawnPoint;

		public ShipSpawner( 
			DiContainer container,
			Ship.Factory factory,
			GameplaySettings.Player settings )
		{
			_factory = factory;
			_spawnPoint = container.ResolveId<IOrientation>( settings.SpawnPointId );
		}

		public Ship Create()
		{
			var newPlayer = _factory.Create();
			newPlayer.transform.SetParent( _spawnPoint.Parent );
			newPlayer.transform.SetPositionAndRotation( _spawnPoint.Position, _spawnPoint.Rotation );

			return newPlayer;
		}
    }
}
