using System;
using Cinemachine;
using Sirenix.OdinInspector;

namespace Minipede.Gameplay.Fx
{
	public class ScreenShakeVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;

		public ScreenShakeVfxAnimator( Settings settings )
		{
			_settings = settings;
		}

		public void Play( IFxSignal signal )
		{
			_settings.Definition.CreateEvent( signal.Position, signal.Direction * _settings.Force );
		}

		[System.Serializable]
		public class Settings : IFxAnimator.ISettings
		{
			public Type AnimatorType => typeof( ScreenShakeVfxAnimator );

			[BoxGroup]
			public CinemachineImpulseDefinition Definition;
			[BoxGroup]
			public float Force;
		}
	}
}