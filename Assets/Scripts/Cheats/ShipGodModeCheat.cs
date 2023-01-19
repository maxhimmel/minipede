using System;
using Minipede.Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Minipede.Cheats
{
	public class ShipGodModeCheat : IInitializable,
		IDisposable
	{
		private readonly PlayerController _playerController;

		public ShipGodModeCheat( PlayerController playerController )
		{
			_playerController = playerController;
		}

		public void Dispose()
		{
			_playerController.PlayerSpawned -= OnShipSpawned;
		}

		public void Initialize()
		{
			_playerController.PlayerSpawned += OnShipSpawned;
		}

		private void OnShipSpawned( Ship ship )
		{
			var colliders = ship.GetComponentsInChildren<Collider2D>();
			foreach ( var collider in colliders )
			{
				collider.enabled = false;
			}
		}
	}
}