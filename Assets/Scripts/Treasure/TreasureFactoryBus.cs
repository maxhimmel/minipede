using System.Collections.Generic;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
	public class TreasureFactoryBus : PooledPrefabFactoryBus<Treasure>
	{
		/// <summary>
		/// This ID should match a <see cref="Transform"/> within the scene being bound using a <see cref="ZenjectBinding"/>.
		/// </summary>
		private const string _containerId = "TreasurePool";

		public TreasureFactoryBus( List<PoolSettings> settings, 
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