using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class ArenaBoundary : MonoBehaviour
    {
		private Collider2D _collider;
		private Bounds _bounds;
		private LayerMask _hitMask;
		private RaycastHit2D _emptyHit2D;

		[Inject]
		public void Construct( Collider2D collider )
		{
            _collider = collider;
			_bounds = _collider.bounds;
			_hitMask = LayerMask.GetMask( LayerMask.LayerToName( gameObject.layer ) );
			_emptyHit2D = new RaycastHit2D();
		}

		public RaycastHit2D Raycast( Vector2 origin, Vector2 direction, float distance, bool checkOverlap = true )
		{
			if ( checkOverlap && !IsOverlapping( origin ) )
			{
				return _emptyHit2D;
			}

			var hitInfo = Physics2D.Raycast( origin, direction, distance, _hitMask );
			if ( hitInfo.collider != _collider )
			{
				return _emptyHit2D;
			}

			return hitInfo;
		}

		public bool IsOverlapping( Vector2 other )
		{
			return _bounds.Contains( other );
		}

		public void SetCollisionActive( bool isActive )
		{
			_collider.attachedRigidbody.simulated = isActive;

			// This caused strange behavior when reactivating.
				// The mosquitos wouldn't detect the arena bounds any longer.
				// No clue why, but the above solves the issue.
			//_collider.enabled = isActive; 
		}
	}
}
