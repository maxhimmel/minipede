using System;
using System.Collections.Generic;
using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.Fx;
using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class Explorer : MonoBehaviour,
		IPawn,
		IDamageController,
		IDisposable
	{
		public event IDamageController.OnHit Damaged {
			add => _damageController.Damaged += value;
			remove => _damageController.Damaged -= value;
		}
		public event IDamageController.OnHit Died {
			add => _damageController.Died += value;
			remove => _damageController.Died -= value;
		}

		public HealthController Health => _damageController.Health;
		public Rigidbody2D Body => _body;
		public IOrientation Orientation => new Orientation( _body.position, _body.transform.rotation, _body.transform.parent );
		public ISelectable CurrentInteractable => _interactionSelector.Selectable;

		private Settings _settings;
		private IDamageController _damageController;
		private IMotor _motor;
		private Rigidbody2D _body;
		private TreasureHauler _treasureHauler;
		private InteractionSelector _interactionSelector;
		private List<TargetGroupAttachment> _targetGroupAttachments;
		private SpriteBlinkVfxAnimator _ejectVfx;
		private SignalBus _signalBus;

		private Vector2 _moveInput;
		private bool _isMoveInputConsumed;
		private bool _isCleanedUp;
		private float _invincibilityEndTime;

		[Inject]
		public void Construct( Settings settings,
			IDamageController damageController,
			IMotor motor,
			Rigidbody2D body,
			TreasureHauler treasureHauler,
			InteractionSelector interactionSelector,
			List<TargetGroupAttachment> targetGroupAttachments,
			SpriteBlinkVfxAnimator ejectVfx,
			SignalBus signalBus )
		{
			_settings = settings;
            _damageController = damageController;
			_motor = motor;
			_body = body;
			_treasureHauler = treasureHauler;
			_interactionSelector = interactionSelector;
			_targetGroupAttachments = targetGroupAttachments;
			_ejectVfx = ejectVfx;
			_signalBus = signalBus;

			damageController.Died += OnDied;
		}

		public void EnterShip( Ship ship )
		{
			_signalBus.FireId( "Pilot", new FxSignal( _body.position, Vector2.up, null ) );
			_treasureHauler.CollectAll( ship.Body );
			Dispose();
		}

		public void StartGrabbing()
		{
			_treasureHauler.StartGrabbing();
		}

		public void StopGrabbing()
		{
			_treasureHauler.StopGrabbing();
		}

		public void ReleaseAllTreasure()
		{
			_treasureHauler.ReleaseAll();
		}

		public void StartReleasingTreasure()
		{
			_treasureHauler.StartReleasingTreasure();
		}

		public void StopReleasingTreasure()
		{
			_treasureHauler.StopReleasingTreasure();
		}

		public bool TryGetFirstHaulable<THaulable>( out THaulable haulable )
			where THaulable : Haulable
		{
			return _treasureHauler.TryGetFirst( out haulable );
		}

		public void ReleaseTreasure( Haulable haulable )
		{
			_treasureHauler.ReleaseTreasure( haulable );
		}

		public int TakeDamage( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
		{
			if ( _invincibilityEndTime > Time.timeSinceLevelLoad )
			{
				return 0;
			}

			return _damageController.TakeDamage( instigator, causer, data );
		}

		private void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			Dispose();
		}

		public void Dispose()
		{
			if ( _isCleanedUp )
			{
				return;
			}

			_damageController.Died -= OnDied;

			foreach ( var targetGroupAttachment in _targetGroupAttachments )
			{
				targetGroupAttachment.Deactivate( canDispose: true ).Forget();
			}

			Destroy( gameObject );
			_isCleanedUp = true;
		}

		public void AddMoveInput( Vector2 input )
		{
			_isMoveInputConsumed = false;
			_moveInput += input;
		}

		private void Update()
		{
			ConsumeDesiredVelocity();
		}

		private void ConsumeDesiredVelocity()
		{
			if ( _isMoveInputConsumed )
			{
				return;
			}

			_moveInput = Vector2.ClampMagnitude( _moveInput, 1 );
			_motor.SetDesiredVelocity( _moveInput );

			_moveInput = Vector2.zero;
			_isMoveInputConsumed = true;
		}

		private void FixedUpdate()
		{
			_motor.FixedTick();

			RotateTowardsVelocity();
		}

		private void RotateTowardsVelocity()
		{
			Vector2 velocity = _motor.Velocity;
			if ( velocity.sqrMagnitude >= 0.01f )
			{
				_body.MoveRotation( velocity.ToLookRotation() );
			}
		}

		public void Eject( Vector2 direction )
		{
			_invincibilityEndTime = Time.timeSinceLevelLoad + _settings.EjectInvincibleDuration;

			_body.AddForce( direction * _settings.EjectForce, ForceMode2D.Impulse );

			_ejectVfx.Play( new FxSignal( transform.position, direction, transform ) );
		}

		[System.Serializable]
		public class Settings
		{
			public float EjectForce;
			public float EjectInvincibleDuration;
			public SpriteBlinkVfxAnimator.Settings EjectInvincibleVfx;
		}

		public class Factory : UnityFactory<Explorer>
		{
			public Factory( DiContainer container, Explorer prefab ) 
				: base( container, prefab )
			{
			}
		}
	}
}
