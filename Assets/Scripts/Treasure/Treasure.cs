using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
    public class Treasure : MonoBehaviour,
		ICleanup
    {
		private Settings _settings;
		private Rigidbody2D _body;

		private bool _isCleanedUp;
		private CancellationTokenSource _cleanupCancelSource;
		private CancellationToken _cleanupCancelToken;

		[Inject]
		public void Construct( Settings settings,
			Rigidbody2D body )
		{
			_settings = settings;
            _body = body;

			_cleanupCancelSource = new CancellationTokenSource();
			_cleanupCancelToken = _cleanupCancelSource.Token;
		}

		public void Launch( Vector2 impulse )
		{
			_body.velocity = impulse;
			_body.angularVelocity = _settings.TorqueRange.Random();

			StartLifetime()
				.Cancellable( _cleanupCancelToken )
				.Forget();
		}

		private async UniTask StartLifetime()
		{
			await TaskHelpers.DelaySeconds( _settings.LifetimeRange.Random(), _cleanupCancelToken );
			Cleanup();
		}

		public void Cleanup()
		{
			if ( _isCleanedUp )
			{
				return;
			}

			_cleanupCancelSource.Cancel();
			_cleanupCancelSource.Dispose();

			Destroy( gameObject );
			_isCleanedUp = true;
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 5, 120 )]
			public Vector2 LifetimeRange;
			[MinMaxSlider( 0, 1080 )]
			public Vector2 TorqueRange;
		}

		public class Factory : UnityPrefabFactory<Treasure> { }
	}
}
