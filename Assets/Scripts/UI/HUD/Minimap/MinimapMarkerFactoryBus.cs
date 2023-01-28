using Minipede.Utility;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class MinimapMarkerFactoryBus : PooledPrefabFactoryBus<MinimapMarker>
	{
		/// <summary>
		/// This ID should match a <see cref="Transform"/> within the scene being bound using a <see cref="ZenjectBinding"/>.
		/// </summary>
		private const string _containerId = "MinimapMarkerPool";

		public MinimapMarkerFactoryBus( List<PoolSettings> settings,
			DiContainer container )
			: base( settings, container )
		{
		}

		protected override Transform GetPoolContainer()
		{
			return _container.ResolveId<RectTransform>( _containerId );
		}
	}
}