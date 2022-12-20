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
		public Rigidbody2D Body => _body;
		public float Weight => _followController.Weight;
		public bool IsFollowing => _followController.IsFollowing;
		public Vector2 Target => _followController.Target;

		private Settings _settings;
		private Rigidbody2D _body;
		private SignalBus _signalBus;
		private IFollower _followController;
		private Lifetimer _lifetimer;

		private bool _isCleanedUp;
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

			_lifetimer = new Lifetimer( settings.LifetimeRange.Random() );

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

			_lifetimer.Reset();
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

			_lifetimer.Pause();
			_followController.SnapToCollector( collector );
		}

		public void Follow( Rigidbody2D target )
		{
			UpdateTether();
			_tetherRenderer.enabled = true;

			_lifetimer.Pause();
			_followController.Follow( target );
		}

		public void StopFollowing()
		{
			_tetherRenderer.enabled = false;

			_lifetimer.Reset();
			_followController.StopFollowing();
		}

		private void FixedUpdate()
		{
			FixedTick();
		}

		public void FixedTick()
		{
			if ( !_lifetimer.Tick() )
			{
				Cleanup();
				return;
			}

			if ( IsFollowing )
			{
				UpdateTether();
			}

			_followController.FixedTick();
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
					otherBody.transform
				) );

				collector.Collect( this );
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
