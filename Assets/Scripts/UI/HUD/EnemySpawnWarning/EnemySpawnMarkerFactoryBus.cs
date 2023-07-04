using System.Collections.Generic;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class EnemySpawnMarkerFactoryBus : PooledPrefabFactoryBus<EnemySpawnMarker>
	{
		/// <summary>
		/// This ID should match a <see cref="Transform"/> within the scene being bound using a <see cref="ZenjectBinding"/>.
		/// </summary>
		private const string _containerId = "EnemySpawnMarkerPool";

		public EnemySpawnMarkerFactoryBus( List<PoolSettings> settings, 
			DiContainer container ) 
			: base( settings, container )
		{
		}

		protected override Transform GetPoolContainer()
		{
			return _container.ResolveId<Transform>( _containerId );
		}
	}
}