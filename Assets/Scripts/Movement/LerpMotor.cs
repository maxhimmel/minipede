using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Movement
{
	public class LerpMotor : IMotor,
		IRemoteMotor
	{
		public bool IsMoving => _lerpTimer < 1 && _travelDuration > 0;
		public Vector2 Velocity => _simluatedVelocity;
		IMotor.ISettings IMotor.Settings => _settings;

		private Settings _settings;
		private readonly IMotor.ISettings _maxSpeedSettings;
		private readonly Rigidbody2D _body;

		private float _lerpTimer;
		private float _travelDuration;
		private Vector2 _startPos;
		private Vector2 _endPos;
		private Vector2 _simluatedVelocity;

		public LerpMotor( Settings settings,
			IMotor.ISettings maxSpeedSettings,
			Rigidbody2D body )
		{
			_settings = settings;
			_maxSpeedSettings = maxSpeedSettings;
			_body = body;

			SetDesiredVelocity( Vector2.zero );
		}

		public void SetMaxSpeed( float maxSpeed )
		{
			_settings.MaxSpeed = maxSpeed;
		}

		public void SetDesiredVelocity( Vector2 direction )
		{
			StartMoving( direction );
		}

		public void StartMoving( Vector2 direction )
		{
			_startPos = _body.position;
			_endPos = _startPos + direction;

			_simluatedVelocity = _endPos - _startPos;
			float travelDistance = _simluatedVelocity.magnitude;
			if ( travelDistance > 0 )
			{
				_simluatedVelocity /= travelDistance * _maxSpeedSettings.MaxSpeed;//_settings.MaxSpeed;
			}

			_lerpTimer = 0;
			_travelDuration = travelDistance / _maxSpeedSettings.MaxSpeed;//_settings.MaxSpeed;
		}

		public void StopMoving()
		{
			_travelDuration = 0;
			_startPos = _endPos = _body.position;

			_simluatedVelocity = Vector2.zero;
		}

		public void FixedTick()
		{
			if ( !IsMoving )
			{
				StopMoving();
				return;
			}

			_lerpTimer += Time.fixedDeltaTime / _travelDuration;
			Vector2 newPos = Vector2.Lerp( _startPos, _endPos, _lerpTimer );

			_body.MovePosition( newPos );
		}

		[System.Serializable]
		public struct Settings : IMotor.ISettings
		{
			float IMotor.ISettings.MaxSpeed => MaxSpeed;

			public float MaxSpeed;
		}
	}
}
