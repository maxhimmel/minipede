using System;
using Cinemachine;
using Sirenix.OdinInspector;

namespace Minipede.Gameplay.Vfx
{
	public class ScreenShakeVfxAnimator : IVfxAnimator
	{
		private readonly Settings _settings;

		public ScreenShakeVfxAnimator( Settings settings )
		{
			_settings = settings;
		}

		public void Play( IVfxSignal signal )
		{
			_settings.Definition.CreateEvent( signal.Position, signal.Direction * _settings.Force );
		}

		[System.Serializable]
		public struct Settings : IVfxAnimator.Settings
		{
			public Type AnimatorType => typeof( ScreenShakeVfxAnimator );

			[BoxGroup]
			public CinemachineImpulseDefinition Definition;
			[BoxGroup]
			public float Force;
		}
	}
}