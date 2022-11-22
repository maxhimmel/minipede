using Minipede.Gameplay.Vfx;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public abstract class VfxListenerSettings<TSignal> : ScriptableObjectInstaller
		where TSignal : IVfxSignal
	{
		[ListDrawerSettings( Expanded = true )]
		[SerializeReference] private IVfxAnimator.Settings[] _vfxSettings;

		public override void InstallBindings()
		{
			Container.DeclareSignal( typeof( TSignal ) )
				.OptionalSubscriber();

			foreach ( var settings in _vfxSettings )
			{
				Container.BindInterfacesAndSelfTo( settings.AnimatorType )
					.AsCached()
					.WithArguments( settings )
					.WhenInjectedInto<VfxListener<TSignal>>();
			}

			Container.BindInterfacesAndSelfTo<VfxListener<TSignal>>()
				.AsSingle();
		}
	}
}