using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay
{
	public class Follower : IFollower
	{
		public float Weight => _settings.Weight;
		public bool IsFollowing => _followMode != FollowMode.None && _followTarget != null;
		public Vector2 Target => _followTarget.position;

		private readonly Settings _settings;
		private readonly Rigidbody2D _body;

		private FollowMode _followMode;
		private Rigidbody2D _followTarget;

		public Follower( Settings settings,
			Rigidbody2D body )
		{
			_settings = settings;
			_body = body;
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
			SetFollowMode( FollowMode.Haul );
		}

		public void StopFollowing()
		{
			_followTarget = null;
			SetFollowMode( FollowMode.None );
		}

		private void SetFollowMode( FollowMode mode )
		{
			_followMode = mode;
		}

		public void FixedTick()
		{
			if ( CanFollow() )
			{
				switch ( _followMode )
				{
					case FollowMode.Haul:
						MoveIntoTargetRadius();
						break;

					case FollowMode.Collect:
						SnapToTarget();
						break;
				}
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

		private void SnapToTarget()
		{
			float moveDelta = Time.deltaTime * _settings.SnapToCollectorSpeed;
			Vector2 newPos = Vector2.MoveTowards( _body.position, _followTarget.position, moveDelta );

			_body.velocity = Vector2.zero;
			_body.MovePosition( newPos );
		}

		[System.Serializable]
		public struct Settings
		{
			public float FollowForce;
			public float SnapToCollectorSpeed;
			public float MinFollowDistance;
			public float Weight;
		}

		private enum FollowMode
		{
			None,

			Haul,
			Collect
		}
	}
}