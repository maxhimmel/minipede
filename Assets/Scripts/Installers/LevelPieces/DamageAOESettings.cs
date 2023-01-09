using Minipede.Gameplay.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Assets.Scripts.Installers.LevelPieces
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Weapons/DamageAOE" )]
	public class DamageAOESettings : ScriptableObjectInstaller
	{
		[HideLabel, BoxGroup]
		[SerializeField] private DamageAOE.Settings _settings;

		public override void InstallBindings()
		{
			Container.BindInstance( _settings )
				.AsSingle()
				.WhenInjectedInto<DamageAOE>();
		}
	}
}