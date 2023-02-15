using Minipede.Gameplay.Player;
using Minipede.Installers;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class ShipSpawner
	{
		public IOrientation SpawnPoint { get; }

		private readonly Ship.Factory _factory;

		public ShipSpawner( 
			DiContainer container,
			Ship.Factory factory,
			PlayerSettings.Player settings )
		{
			_factory = factory;
			SpawnPoint = container.ResolveId<IOrientation>( settings.SpawnPointId );
		}

		public Ship Create()
		{
			var newPlayer = _factory.Create();
			newPlayer.transform.SetParent( SpawnPoint.Parent );
			newPlayer.transform.SetPositionAndRotation( SpawnPoint.Position, SpawnPoint.Rotation );

			return newPlayer;
		}
    }
}
