using System.Collections.Generic;
using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Collectables/TreasurePoolSettings" )]
	public class TreasurePoolSettings : ScriptableObjectInstaller
	{
		[SerializeField] private List<TreasureFactoryBus.PoolSettings> _treasures;

		public override void InstallBindings()
		{
			Container.Bind<TreasureFactoryBus>()
				.AsSingle()
				.WithArguments( _treasures );
		}
	}
}