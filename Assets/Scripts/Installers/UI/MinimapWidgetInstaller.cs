using System.Collections.Generic;
using Minipede.Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class MinimapWidgetInstaller : MonoInstaller
	{
		[SerializeField] private RectTransform _markerContainer;
		[SerializeField] private MinimapMarkerFactoryBus.Settings _minimapMarkers;

		public override void InstallBindings()
		{
			Container.BindInstance( _markerContainer )
				.WithId( "MinimapMarkerPool" );

			Container.BindInterfacesAndSelfTo<MinimapMarkerFactoryBus>()
				.AsSingle()
				.WithArguments( _minimapMarkers );
		}
	}
}