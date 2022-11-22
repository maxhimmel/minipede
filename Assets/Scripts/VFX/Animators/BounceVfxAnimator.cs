using System;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Vfx;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Assets.Scripts.VFX.Animators
{
	public class BounceVfxAnimator : IVfxAnimator
	{
		private readonly Settings _settings;
		private readonly BounceScaler _bouncer;

		public BounceVfxAnimator( Settings settings,
			SpriteRenderer renderer )
		{
			_settings = settings;
			_bouncer = new BounceScaler( renderer );
		}

		public void Play( IVfxSignal signal )
		{
			_bouncer.Bounce( _settings.BounceSettings )
				.Forget();
		}

		[System.Serializable]
		public struct Settings : IVfxAnimator.Settings
		{
			public Type AnimatorType => typeof( BounceVfxAnimator );

			[HideLabel]
			public BounceScaler.Settings BounceSettings;
		}
	}
}