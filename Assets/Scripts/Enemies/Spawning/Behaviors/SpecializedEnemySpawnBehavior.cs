namespace Minipede.Gameplay.Enemies.Spawning
{
	public abstract class SpecializedEnemySpawnBehavior<TEnemy> : EnemySpawnBehavior
		where TEnemy : EnemyController
	{
		public override System.Type EnemyType => typeof( TEnemy );

		public override void Perform( EnemyController enemy )
		{
			base.Perform( enemy );
			HandleSpecialtySpawn( enemy as TEnemy );
		}

		/// <summary>
		/// This is called after <see cref="EnemyController.OnSpawned"/> has been invoked.
		/// </summary>
		protected abstract void HandleSpecialtySpawn( TEnemy newEnemy );
	}
}
