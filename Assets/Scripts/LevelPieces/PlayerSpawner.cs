using Minipede.Gameplay.Player;
using Minipede.Installers;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class PlayerSpawner
	{
		private readonly PlayerController.Factory _factory;
		private readonly Transform _spawnPoint;

		public PlayerSpawner( 
			DiContainer container,
			PlayerController.Factory factory,
			GameplaySettings.Player settings )
		{
			_factory = factory;
			_spawnPoint = container.ResolveId<Transform>( settings.SpawnPointId );
		}

		public PlayerController Spawn()
		{
			var newPlayer = _factory.Create();
			newPlayer.transform.SetParent( null );
			newPlayer.transform.position = _spawnPoint.position;

			return newPlayer;
		}
    }
}
