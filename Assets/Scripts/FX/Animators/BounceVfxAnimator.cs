using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Fx
{
	public class BounceVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly BounceScaler _bouncer;

		public BounceVfxAnimator( Settings settings,
			SpriteRenderer renderer )
		{
			_settings = settings;
			_bouncer = new BounceScaler( renderer );
		}

		public void Play( IFxSignal signal )
		{
			_bouncer.Bounce( _settings.BounceSettings )
				.Forget();
		}

		[System.Serializable]
		public class Settings : IFxAnimator.ISettings
		{
			public Type AnimatorType => typeof( BounceVfxAnimator );

			[HideLabel]
			public BounceScaler.Settings BounceSettings;
		}
	}
}