using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Movement
{
	public class SineMotor : IMotor,
		IRemoteMotor
	{
		public bool IsMoving => _sineDirection.sqrMagnitude > 0.01f || Velocity.sqrMagnitude > 0.01f;
		public Vector2 Velocity => _velocity;

		private readonly Settings _settings;
		private readonly IMaxSpeed _maxSpeed;
		private readonly Rigidbody2D _body;

		private Vector2 _origin;
		private Vector2 _sineDirection;
		private Vector2 _velocity;
		private Vector2 _currentDirection;
		private float _sineTimer;

		public SineMotor( Settings settings,
			IMaxSpeed maxSpeedSettings,
			Rigidbody2D body )
		{
			_settings = settings;
			_maxSpeed = maxSpeedSettings;
			_body = body;

			_origin = body.position;
		}

		public void StartMoving( Vector2 direction )
		{
			_sineTimer = 0;
			_origin = _body.position;

			SetDesiredVelocity( direction );
		}

		public void StopMoving()
		{
			SetDesiredVelocity( Vector2.zero );
		}

		public void RecalibrateVelocity()
		{
			SetDesiredVelocity( _currentDirection );
		}

		public void SetDesiredVelocity( Vector2 direction )
		{
			_currentDirection = direction;
			_sineDirection = direction.Rotate( 90 );
			_velocity = direction * _maxSpeed.GetMaxSpeed();
		}

		public void FixedTick()
		{
			Vector2 newPos = UpdatePosition();
			_body.MovePosition( newPos );
		}

		private Vector2 UpdatePosition()
		{
			Vector2 velocityDelta = _velocity * Time.fixedDeltaTime;

			Vector2 newPos = _origin
				+ _sineDirection * UpdateSine()
				+ velocityDelta;

			_origin += velocityDelta;

			return newPos;
		}

		private float UpdateSine()
		{
			_sineTimer += Time.fixedDeltaTime;

			return _settings.Evaluate( _sineTimer, _maxSpeed.GetMaxSpeed() );
		}

		[System.Serializable]
		public struct Settings : IMaxSpeed
		{
			[BoxGroup( "Speed", ShowLabel = false )]
			public float MaxSpeed;

			[ToggleGroup( "ScaleBySpeed", GroupID = "Speed/ScaleBySpeed" )]
			public bool ScaleBySpeed;
			[HorizontalGroup( "Speed/ScaleBySpeed/Matching" )]
			public bool MatchLinearSpeed;
			[HorizontalGroup( "Speed/ScaleBySpeed/Matching" ), DisableIf( "MatchLinearSpeed" )]
			public float MaxWaveSpeed;

			[BoxGroup( "Speed/Wave" ), HideLabel]
			public WaveDatum Wave;

			private float? _currentMaxSpeed;

			public float GetMaxSpeed()
			{
				return _currentMaxSpeed.HasValue
					? _currentMaxSpeed.Value
					: MaxSpeed;
			}

			public void SetMaxSpeed( float maxSpeed )
			{
				_currentMaxSpeed = maxSpeed;
			}

			public void RestoreMaxSpeed()
			{
				_currentMaxSpeed = null;
			}

			public float Evaluate( float timer )
			{
				return Evaluate( timer, GetMaxSpeed() );
			}

			public float Evaluate( float timer, float maxSpeedOverride )
			{
				if ( !ScaleBySpeed )
				{
					return Wave.Evaluate( timer );
				}

				float totalWaveDistanceTravelled = Wave.Amplitude * 4;
				float moveSpeed = MatchLinearSpeed ? maxSpeedOverride : MaxWaveSpeed;
				float moveSpeedScalar = totalWaveDistanceTravelled / moveSpeed;

				return Wave.Evaluate( timer / moveSpeedScalar );
			}
		}
	}
}
