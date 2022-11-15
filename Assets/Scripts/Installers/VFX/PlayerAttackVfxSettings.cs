using Minipede.Gameplay.Vfx;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = "VFX/Player Attack" )]
    public class PlayerAttackVfxSettings : ScriptableObjectInstaller
    {
        [HideLabel]
        [SerializeField] private PlayerAttackVfx.Settings _settings;

		public override void InstallBindings()
		{
            Container.BindInterfacesAndSelfTo<PlayerAttackVfx>()
                .AsSingle()
                .WithArguments( _settings );
		}
	}
}
