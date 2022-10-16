using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Movement
{
	public class SineMotor : IMotor,
		IRemoteMotor
	{
		public Vector2 Velocity => _desiredVelocity;

		private readonly Settings _settings;
		private readonly Rigidbody2D _body;

		private Vector2 _origin;
		private Vector2 _sineDirection;
		private Vector2 _desiredVelocity;
		private float _sineTimer;

		public SineMotor( Settings settings,
			Rigidbody2D body )
		{
			_settings = settings;
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

		public void SetDesiredVelocity( Vector2 direction )
		{
			_sineDirection = direction.Rotate( 90 );
			_desiredVelocity = direction * _settings.MaxSpeed;
		}

		public void FixedTick()
		{
			Vector2 newPos = UpdatePosition();
			_body.MovePosition( newPos );
		}

		private Vector2 UpdatePosition()
		{
			Vector2 velocityDelta = _desiredVelocity * Time.fixedDeltaTime;

			Vector2 newPos = _origin
				+ _sineDirection * UpdateSine()
				+ velocityDelta;

			_origin += velocityDelta;

			return newPos;
		}

		private float UpdateSine()
		{
			_sineTimer += Time.fixedDeltaTime;

			return _settings.Evaluate( _sineTimer );
		}

		[System.Serializable]
		public struct Settings
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

			public float Evaluate( float timer )
			{
				if ( !ScaleBySpeed )
				{
					return Wave.Evaluate( timer );
				}

				float totalWaveDistanceTravelled = Wave.Amplitude * 4;
				float moveSpeed = MatchLinearSpeed ? MaxSpeed : MaxWaveSpeed;
				float moveSpeedScalar = totalWaveDistanceTravelled / moveSpeed;

				return Wave.Evaluate( timer / moveSpeedScalar );
			}
		}
	}
}
