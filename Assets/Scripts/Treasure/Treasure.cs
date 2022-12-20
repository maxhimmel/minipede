using Minipede.Gameplay.Fx;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
    public abstract class Treasure : MonoBehaviour,
		IFollower,
		ICleanup
    {
		public float Weight => _followController.Weight;
		public bool IsFollowing => _followController.IsFollowing;
		public Vector2 Target => _followController.Target;

		private Settings _settings;
		private Rigidbody2D _body;
		private SignalBus _signalBus;
		private IFollower _followController;

		private bool _isCleanedUp;
		private float _lifetimer;
		private float _lifetimeDuration;
		private LineRenderer _tetherRenderer;
		private Vector3[] _tetherPositions;

		[Inject]
		public void Construct( Settings settings,
			Rigidbody2D body,
			SignalBus signalBus,
			IFollower followController )
		{
			_settings = settings;
            _body = body;
			_signalBus = signalBus;
			_followController = followController;

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
			_followController.SnapToCollector( collector );
		}

		public void Follow( Rigidbody2D target )
		{
			UpdateTether();
			_tetherRenderer.enabled = true;

			_followController.Follow( target );
		}

		public void StopFollowing()
		{
			_tetherRenderer.enabled = false;

			_followController.StopFollowing();
		}

		private void FixedUpdate()
		{
			FixedTick();
		}

		public void FixedTick()
		{
			_followController.FixedTick();

			if ( IsFollowing )
			{
				UpdateTether();
			}
		}

		private void UpdateTether()
		{
			if ( _tetherRenderer.enabled )
			{
				_tetherPositions[0] = _body.position;
				_tetherPositions[1] = Target;
				_tetherRenderer.SetPositions( _tetherPositions );
			}
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
			if ( IsFollowing )
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
			[MinMaxSlider( 5, 120 )]
			public Vector2 LifetimeRange;
			[MinMaxSlider( 0, 1080 )]
			public Vector2 TorqueRange;

			[Space]
			public LineRenderer TetherPrefab;
		}

		public class Factory : UnityPrefabFactory<Treasure> { }
	}
}
