using System.Collections.Generic;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Weapons;
using Minipede.Utility;
using UnityEngine;
using UnityEngine.Animations;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
	public class Beacon : Collectable<Beacon>,
		IPushable
	{
		public ICleansedAreaProvider CleansedAreaProvider { get; private set; }
		public Gun Gun { get; private set; }
		Transform IPushable.transform => _owner.transform;

		private IGunProvider _gunProvider;
		private Gun.Factory _gunFactory;
		private PositionConstraint _constraint;
		private List<Collider2D> _colliders = new List<Collider2D>( 1 );
		private Rigidbody2D _owner;

		[Inject]
		public void Construct( ICleansedAreaProvider cleansedAreaProvider,
			IGunProvider gunProvider,
			Gun.Factory gunFactory,
			PositionConstraint constraint )
		{
			CleansedAreaProvider = cleansedAreaProvider;
			_gunProvider = gunProvider;
			_gunFactory = gunFactory;
			_constraint = constraint;
		}

		private void Awake()
		{
			_body.GetAttachedColliders( _colliders );

			Gun = _gunFactory.Create( _gunProvider.GetAsset() );
			Gun.Emptied += ( gun, ammo ) => Dispose();
		}

		public void Equip( Rigidbody2D owner )
		{
			_owner = owner;
			Gun.SetOwner( owner.transform );

			_lifetimer.Pause();

			SetCollidersEnabled( false );
			ClearVelocity();
			ClampOrientation( owner );
		}

		public void Unequip()
		{
			_owner = null;
			Gun.SetOwner( null );

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

		public void Push( Vector2 velocity )
		{
			IPushable pushable = _owner.GetComponent<IPushable>();
			pushable?.Push( velocity );
		}

		public override void FixedTick()
		{
			base.FixedTick();

			if ( _owner == null )
			{
				Gun.FixedTick();
			}
		}

		public class Factory : UnityFactory<Beacon>
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