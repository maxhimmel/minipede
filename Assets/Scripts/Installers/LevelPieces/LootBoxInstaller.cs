using Minipede.Gameplay.Treasures;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = "Loot Box" )]
	public class LootBoxInstaller : ScriptableObjectInstaller
	{
		[HideLabel, BoxGroup]
		[SerializeField] private LootBox.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<LootBox>()
				.AsSingle()
				.WithArguments( _settings );
		}
	}
}