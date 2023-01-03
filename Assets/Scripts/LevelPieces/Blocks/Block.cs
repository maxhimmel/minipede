using Minipede.Installers;
using Minipede.Utility;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class Block : MonoBehaviour,
		IDamageController,
		ICleanup
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

		private IDamageController _damageController;
		private LevelGraph _levelGraph;
		private SignalBus _signalBus;

		private bool _isCleanedUp;

		[Inject]
		public void Construct( IDamageController damageController,
			LevelGraph levelGraph,
			SignalBus signalBus )
		{
			_damageController = damageController;
			_levelGraph = levelGraph;
			_signalBus = signalBus;

			damageController.Died += HandleDeath;
		}

		public int TakeDamage( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
		{
			OnTakeDamage( instigator, causer, data );
			return _damageController.TakeDamage( instigator, causer, data );
		}

		protected virtual void OnTakeDamage( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
		{
		}

		protected virtual void HandleDeath( Rigidbody2D victimBody, HealthController health )
		{
			Cleanup();
		}

		public void Cleanup()
		{
			if ( _isCleanedUp )
			{
				return;
			}

			_isCleanedUp = true;

			HandleCleanup();

			_damageController.Died -= HandleDeath;

			_levelGraph.RemoveBlock( this );

			_signalBus.TryFire( new BlockDestroyedSignal() { Victim = this } );
		}

		protected virtual void HandleCleanup()
		{
			Destroy( gameObject );
		}

		public class Factory : UnityPrefabFactory<Block>
		{
			[Inject]
			private readonly GameplaySettings.Level _settings;

			[Inject]
			private readonly SignalBus _signalBus;

			public override Block Create( Object prefab, IOrientation placement, IEnumerable<object> extraArgs = null )
			{
				var newBlock = base.Create( prefab, placement, extraArgs );

				newBlock.transform.localScale = new Vector3(
					_settings.Graph.Size.x,
					_settings.Graph.Size.y,
					1
				);

				_signalBus.TryFire( new BlockSpawnedSignal()
				{
					NewBlock = newBlock
				} );

				return newBlock;
			}
		}
	}
}
