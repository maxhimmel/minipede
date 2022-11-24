using Minipede.Gameplay.Vfx;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = "FX Listener" )]
	public class FxListenerSettings : ScriptableObjectInstaller
	{
		private bool IsFxIdValid => !string.IsNullOrEmpty( _fxId );

		[InfoBox( "Fx ID must be set.", VisibleIf = "@!IsFxIdValid", InfoMessageType = InfoMessageType.Error )]
		[SerializeField] private string _fxId;

		[ShowIf( "IsFxIdValid" )]
		[ListDrawerSettings( Expanded = true )]
		[SerializeReference] private IFxAnimator.Settings[] _fxSettings;

		public override void InstallBindings()
		{
			DeclareSignal();
			BindAnimators();
			BindListener();
		}

		private void DeclareSignal()
		{
			Container.DeclareSignal<FxSignal>()
				.WithId( _fxId )
				.OptionalSubscriber();
		}

		private void BindAnimators()
		{
			foreach ( var settings in _fxSettings )
			{
				Container.Bind<IFxAnimator>()
					.WithId( _fxId )
					.To( settings.AnimatorType )
					.AsCached()
					.WithArguments( settings );
			}

			if ( !Container.HasBinding<FxAnimatorResolver>() )
			{
				Container.Bind<FxAnimatorResolver>()
					.AsSingle();
			}
		}

		private void BindListener()
		{
			Container.BindInterfacesAndSelfTo<FxListener>()
				.AsCached()
				.WithArguments( _fxId );
		}
	}
}