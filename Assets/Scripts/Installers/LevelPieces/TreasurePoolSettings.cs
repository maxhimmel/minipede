using System.Collections.Generic;
using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Collectables/TreasurePoolSettings" )]
	public class TreasurePoolSettings : ScriptableObjectInstaller
	{
		[SerializeField] private TreasureFactoryBus.Settings _treasures;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<TreasureFactoryBus>()
				.AsSingle()
				.WithArguments( _treasures );
		}
	}
}