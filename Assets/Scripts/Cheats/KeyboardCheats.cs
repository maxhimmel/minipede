using Minipede.Gameplay;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Cheats
{
	public class KeyboardCheats : ITickable
	{
		private readonly Action[] _settings;

		public KeyboardCheats( DiContainer container, 
			Action[] settings )
		{
			_settings = settings;

			foreach ( var action in settings )
			{
				action.Initialize( container );
			}
		}

		public void Tick()
		{
			foreach ( var cheat in _settings )
			{
				if ( Input.GetKeyDown( cheat.Key ) )
				{
					cheat.PerformAction();
				}
			}
		}

		[System.Serializable]
		public abstract class Action
		{
			public KeyCode Key;

			protected DiContainer _container;

			[EnableIf( "@UnityEngine.Application.isPlaying" )]
			[Button( ButtonSizes.Large, Name = "@GetActionName()" )]
			public abstract void PerformAction();

			protected abstract string GetActionName();

			public void Initialize( DiContainer container )
			{
				_container = container;
			}
		}
	}

	[System.Serializable]
	public class KillShipAction : KeyboardCheats.Action
	{
		public override void PerformAction()
		{
			var ship = GameObject.FindObjectOfType<Ship>();
			if ( ship != null )
			{
				ship.TakeDamage( ship.transform, ship.transform, new KillInvoker.Settings() );
			}
		}

		protected override string GetActionName()
		{
			return "Kill Ship";
		}
	}

	[System.Serializable]
	public class SpawnEnemyAction : KeyboardCheats.Action
	{
		[SerializeField] private Enemy _enemy;

		public override void PerformAction()
		{
			var spawnBuilder = _container.Resolve<EnemySpawnBuilder>();

			switch ( _enemy )
			{
				case Enemy.Bee:
					spawnBuilder.Build<BeeController>()
						//.WithPlacement( transform.ToData() )
						.WithRandomPlacement()
						.WithSpawnBehavior()
						.Create();
					return;

				case Enemy.Beetle:
					spawnBuilder.Build<BeetleController>()
						//.WithPlacement( transform.ToData() )
						.WithRandomPlacement()
						.WithSpawnBehavior()
						.Create();
					return;

				case Enemy.Dragonfly:
					spawnBuilder.Build<DragonflyController>()
						//.WithPlacement( transform.ToData() )
						.WithRandomPlacement()
						.WithSpawnBehavior()
						.Create();
					return;

				case Enemy.Earwig:
					spawnBuilder.Build<EarwigController>()
						//.WithPlacement( transform.ToData() )
						.WithRandomPlacement()
						.WithSpawnBehavior()
						.Create();
					return;

				case Enemy.Inchworm:
					spawnBuilder.Build<InchwormController>()
						//.WithPlacement( transform.ToData() )
						.WithRandomPlacement()
						.WithSpawnBehavior()
						.Create();
					return;

				case Enemy.Minipede:
					spawnBuilder.Build<MinipedeController>()
						//.WithPlacement( transform.ToData() )
						.WithRandomPlacement()
						.WithSpawnBehavior()
						.Create();
					return;

				case Enemy.Mosquito:
					spawnBuilder.Build<MosquitoController>()
						//.WithPlacement( transform.ToData() )
						.WithRandomPlacement()
						.WithSpawnBehavior()
						.Create();
					return;

				case Enemy.Spider:
					spawnBuilder.Build<SpiderController>()
						//.WithPlacement( transform.ToData() )
						.WithRandomPlacement()
						.WithSpawnBehavior()
						.Create();
					return;
			}
		}

		protected override string GetActionName()
		{
			return "Spawn Enemy";
		}

		private enum Enemy
		{
			Bee,
			Beetle,
			Dragonfly,
			Earwig,
			Inchworm,
			Minipede,
			Mosquito,
			Spider
		}
	}
}
