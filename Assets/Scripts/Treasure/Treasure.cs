using Minipede.Gameplay.Fx;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
    public abstract class Treasure : MonoBehaviour,
		ICleanup
    {
		public float Weight => _settings.Weight;

		private Settings _settings;
		private Rigidbody2D _body;
		private SignalBus _signalBus;

		private bool _isCleanedUp;
		private float _lifetimer;
		private float _lifetimeDuration;
		private FollowMode _followMode;
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

			_lifetimeDuration = settings.LifetimeRange.Random();

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
		}

		public void Cleanup()
		{
			if ( _isCleanedUp )
			{
				return;
			}

			StopFollowing();

			Destroy( gameObject );
			_isCleanedUp = true;
		}

		public void SnapToCollector( Rigidbody2D collector )
		{
			StopFollowing();

			_followTarget = collector;
			SetFollowMode( FollowMode.Collect );
		}

		public void Follow( Rigidbody2D target )
		{
			_followTarget = target;

			UpdateTether();
			_tetherRenderer.enabled = true;

			SetFollowMode( FollowMode.Haul );
		}

		public void StopFollowing()
		{
			_followTarget = null;
			_tetherRenderer.enabled = false;
			_lifetimer = 0;

			SetFollowMode( FollowMode.None );
		}

		private void SetFollowMode( FollowMode mode )
		{
			_followMode = mode;
		}

		private void FixedUpdate()
		{
			if ( !CanFollow() )
			{
				return;
			}

			switch ( _followMode )
			{
				case FollowMode.Haul:
					MoveIntoTargetRadius();
					UpdateTether();
					break;

				case FollowMode.Collect:
					SnapToTarget();
					break;
			}
		}

		private bool CanFollow()
		{
			return _followMode != FollowMode.None 
				&& _followTarget != null;
		}

		private void MoveIntoTargetRadius()
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

		private void SnapToTarget()
		{
			float moveDelta = Time.deltaTime * _settings.SnapToCollectorSpeed;
			Vector2 newPos = Vector2.MoveTowards( _body.position, _followTarget.position, moveDelta );

			_body.velocity = Vector2.zero;
			_body.MovePosition( newPos );
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

		private void Update()
		{
			CountdownLifetime();
		}

		private void CountdownLifetime()
		{
			if ( _followMode != FollowMode.None )
			{
				return;
			}

			_lifetimer += Time.deltaTime / _lifetimeDuration;
			if ( _lifetimer >= 1 )
			{
				Cleanup();
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
			public float SnapToCollectorSpeed;
			[BoxGroup( "Hauling" )]
			public float MinFollowDistance;
			[BoxGroup( "Hauling" )]
			public float Weight;
		}

		private enum FollowMode
		{
			None,

			Haul,
			Collect
		}

		public class Factory : UnityPrefabFactory<Treasure> { }
	}
}
