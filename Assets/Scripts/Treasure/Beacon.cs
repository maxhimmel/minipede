using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
	public class Beacon : Collectable<Beacon>
	{
		private PositionConstraint _constraint;
		private Light2D _light;
		private List<Collider2D> _colliders = new List<Collider2D>( 1 );

		[Inject]
		public void Construct( PositionConstraint constraint,
			Light2D light )
		{
			_constraint = constraint;
			_light = light;

			_body.GetAttachedColliders( _colliders );
		}

		public void Equip( Rigidbody2D owner )
		{
			_lifetimer.Pause();
			_light.enabled = false;

			SetCollidersEnabled( false );
			ClearVelocity();
			ClampOrientation( owner );
		}

		public void UnEquip()
		{
			_light.enabled = true;

			SetCollidersEnabled( true );
			ReleaseEquipClamping();
		}

		private void SetCollidersEnabled( bool isEnabled )
		{
			foreach ( var collider in _colliders )
			{
				collider.enabled = isEnabled;
			}
		}

		private void ClearVelocity()
		{
			_body.velocity = Vector2.zero;
			_body.angularVelocity = 0;
		}

		private void ClampOrientation( Rigidbody2D owner )
		{
			_body.MovePosition( owner.position );
			_body.MoveRotation( 0 );

			_constraint.AddSource( new ConstraintSource()
			{
				sourceTransform = owner.transform,
				weight = 1
			} );
			_constraint.constraintActive = true;
		}

		private void ReleaseEquipClamping()
		{
			if ( _constraint.constraintActive )
			{
				_constraint.RemoveSource( 0 );
				_constraint.constraintActive = false;
			}
		}

		protected override Beacon GetCollectable()
		{
			return this;
		}
	}
}