using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class MinimapMarkerFactoryBus : PooledPrefabFactoryBus<MinimapMarker>
	{
		/// <summary>
		/// This ID should match what's inside <see cref="Installers.MinimapWidgetInstaller"/>.
		/// </summary>
		private const string _containerId = "MinimapMarkerPool";

		public MinimapMarkerFactoryBus( Settings settings,
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