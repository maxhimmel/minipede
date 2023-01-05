using System.Collections.Generic;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
	public class Beacon : Collectable<Beacon>
	{
		public ICleansedAreaProvider CleansedAreaProvider { get; private set; }

		private PositionConstraint _constraint;
		private Light2D _light;
		private List<Collider2D> _colliders = new List<Collider2D>( 1 );

		[Inject]
		public void Construct( ICleansedAreaProvider cleansedAreaProvider,
			PositionConstraint constraint,
			Light2D light )
		{
			CleansedAreaProvider = cleansedAreaProvider;
			_constraint = constraint;
			_light = light;
		}

		private void Awake()
		{
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

		public void Unequip()
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

		public new class Factory : UnityFactory<Beacon> 
		{
			public ResourceType Resource { get; }

			public Factory( DiContainer container, 
				Beacon prefab,
				ResourceType resource ) 
				: base( container, prefab )
			{
				Resource = resource;
			}
		}
	}
}