using Minipede.Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class MinimapMarkerInstaller : MonoInstaller
    {
		[SerializeField] private MinimapMarker _marker;

		public override void InstallBindings()
		{
			Container.BindInstance( _marker )
				.AsSingle();
		}
	}
}
