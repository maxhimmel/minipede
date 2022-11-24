using Minipede.Gameplay.Vfx;
using Minipede.Gameplay.VFX;
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

	[CreateAssetMenu( menuName = "VFX Listener" )]
	public class VfxListenerSettings : ScriptableObjectInstaller
	{
		[InfoBox( "Vfx ID must be set.", VisibleIf = "@string.IsNullOrEmpty(_vfxId)", InfoMessageType = InfoMessageType.Error )]
		[SerializeField] private string _vfxId;

		[HideIf( "@string.IsNullOrEmpty(_vfxId)" )]
		[ListDrawerSettings( Expanded = true )]
		[SerializeReference] private IVfxAnimator.Settings[] _vfxSettings;

		public override void InstallBindings()
		{
			DeclareSignal();
			BindAnimators();
			BindListener();
		}

		private void DeclareSignal()
		{
			Container.DeclareSignal<FxSignal>()
				.WithId( _vfxId )
				.OptionalSubscriber();
		}

		private void BindAnimators()
		{
			foreach ( var settings in _vfxSettings )
			{
				Container.Bind<IVfxAnimator>()
					.WithId( _vfxId )
					.To( settings.AnimatorType )
					.AsCached()
					.WithArguments( settings );
			}

			if ( !Container.HasBinding<VfxAnimatorResolver>() )
			{
				Container.Bind<VfxAnimatorResolver>()
					.AsSingle();
			}
		}

		private void BindListener()
		{
			Container.BindInterfacesAndSelfTo<VfxListener>()
				.AsCached()
				.WithArguments( _vfxId );
		}
	}
}