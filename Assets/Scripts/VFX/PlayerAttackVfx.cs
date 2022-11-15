using System;
using Minipede.Gameplay.Weapons;
using Sirenix.OdinInspector;
using Zenject;

namespace Minipede.Gameplay.Vfx
{
	public class PlayerAttackVfx :
		IInitializable,
		IDisposable
	{
		private readonly Settings _settings;
		private readonly SignalBus _signalBus;
		private readonly ScreenBlinkController _screenBlinker;

		public PlayerAttackVfx( Settings settings,
			SignalBus signalBus,
			ScreenBlinkController screenBlinker )
		{
			_settings = settings;
			_signalBus = signalBus;
			_screenBlinker = screenBlinker;
		}

		public void Initialize()
		{
			_signalBus.Subscribe<AttackedSignal>( OnAttacked );
		}

		public void Dispose()
		{
			_signalBus.Unsubscribe<AttackedSignal>( OnAttacked );
		}

		private void OnAttacked( AttackedSignal attack )
		{
			_screenBlinker.Blink( _settings.AttackBlink );

			_settings.Recoil.CreateEvent( attack.Position, attack.Direction * _settings.RecoilForce );
		}

		[System.Serializable]
		public struct Settings
		{
			public ScreenBlinkController.Settings AttackBlink;

			[FoldoutGroup( "Recoil" ), HideLabel]
			public Cinemachine.CinemachineImpulseDefinition Recoil;
			[FoldoutGroup( "Recoil" )]
			public float RecoilForce;
		}
	}
}
