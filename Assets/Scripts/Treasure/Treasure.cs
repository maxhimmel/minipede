using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Fx;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
    public class Treasure : MonoBehaviour,
		ICleanup
    {
		public float Weight => _settings.Weight;

		private Settings _settings;
		private Rigidbody2D _body;
		private SignalBus _signalBus;

		private bool _isCleanedUp;
		private CancellationTokenSource _cleanupCancelSource;
		private CancellationToken _cleanupCancelToken;
		private Rigidbody2D _followTarget;
		private LineRenderer _tetherRenderer;
		private Vector3[] _tetherPositions;

		[Inject]
		public void Construct( Settings settings,
			Rigidbody2D body,
			SignalBus signalBus )
		{
			_settings = settings;
            _body = body;
			_signalBus = signalBus;

			_cleanupCancelSource = new CancellationTokenSource();
			_cleanupCancelToken = _cleanupCancelSource.Token;

			SetupTether();
		}

		private void SetupTether()
		{
			_tetherPositions = new Vector3[2];

			_tetherRenderer = Instantiate( _settings.TetherPrefab, transform );
			_tetherRenderer.positionCount = 2;
			_tetherRenderer.enabled = false;
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

			_tetherRenderer.enabled = false;

			_cleanupCancelSource.Cancel();
			_cleanupCancelSource.Dispose();

			Destroy( gameObject );
			_isCleanedUp = true;
		}

		public void Follow( Rigidbody2D target )
		{
			_followTarget = target;

			UpdateTether();
			_tetherRenderer.enabled = true;
		}

		public void StopFollowing()
		{
			_followTarget = null;
			_tetherRenderer.enabled = false;
		}

		private void FixedUpdate()
		{
			if ( !CanFollow() )
			{
				return;
			}

			MoveTowardsTarget();
			UpdateTether();
		}

		private bool CanFollow()
		{
			return _followTarget != null;
		}

		private void MoveTowardsTarget()
		{
			Vector2 selfToTarget = _followTarget.position - _body.position;
			if ( selfToTarget.sqrMagnitude < _settings.MinFollowDistance * _settings.MinFollowDistance )
			{
				return;
			}

			_body.AddForce( selfToTarget.normalized * _settings.FollowForce, ForceMode2D.Force );
		}

		private void UpdateTether()
		{
			_tetherPositions[0] = _body.position;
			_tetherPositions[1] = _followTarget.position;
			_tetherRenderer.SetPositions( _tetherPositions );
		}

		private void OnCollisionEnter2D( Collision2D collision )
		{
			var otherBody = collision.rigidbody;
			ICollector collector = otherBody?.GetComponent<ICollector>();
			if ( collector != null )
			{
				_signalBus.FireId( "Collected", new FxSignal(
					_body.position,
					(otherBody.position - _body.position).normalized
				) );

				collector.Collect( this );
			}
		}

		[System.Serializable]
		public struct Settings
		{
			[BoxGroup( "Spawning" ), MinMaxSlider( 5, 120 )]
			public Vector2 LifetimeRange;
			[BoxGroup( "Spawning" ), MinMaxSlider( 0, 1080 )]
			public Vector2 TorqueRange;

			[BoxGroup( "Hauling" )]
			public LineRenderer TetherPrefab;
			[BoxGroup( "Hauling" )]
			public float FollowForce;
			[BoxGroup( "Hauling" )]
			public float MinFollowDistance;
			[BoxGroup( "Hauling" )]
			public float Weight;
		}

		public class Factory : UnityPrefabFactory<Treasure> { }
	}
}
