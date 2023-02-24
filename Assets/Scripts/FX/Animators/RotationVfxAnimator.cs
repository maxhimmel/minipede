using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Fx
{
	public class RotationVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly SpriteRenderer _renderer;
		private readonly Quaternion _initialLocalRotation;
		private readonly CancellationToken _destroyCancelToken;

		public RotationVfxAnimator( Settings settings,
			SpriteRenderer renderer )
		{
			_settings = settings;
			_renderer = renderer;
			_initialLocalRotation = renderer.transform.localRotation;
			_destroyCancelToken = renderer.GetCancellationTokenOnDestroy();
		}

		public void Play( IFxSignal signal )
		{
			Play().Cancellable( _destroyCancelToken );
		}

		private async UniTask Play()
		{
			float randAngle = _settings.AngleRange.Random();
			_renderer.transform.localRotation *= Quaternion.Euler( 0, 0, randAngle );

			await TaskHelpers.DelaySeconds( _settings.Duration, _destroyCancelToken );

			_renderer.transform.localRotation = _initialLocalRotation;
		}

		[System.Serializable]
		public class Settings : IFxAnimator.ISettings
		{
			public Type AnimatorType => typeof( RotationVfxAnimator );

			[MinMaxSlider( -180, 180, ShowFields = true )]
			public Vector2 AngleRange;

			public float Duration;
		}
	}
}
