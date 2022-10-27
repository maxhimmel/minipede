namespace Minipede.Gameplay.Enemies.Spawning
{
	public abstract class SpecializedEnemySpawner<TEnemy> : EnemySpawner
		where TEnemy : EnemyController
	{
		public override System.Type EnemyType => typeof( TEnemy );

		protected override void OnSpawned( EnemyController newEnemy )
		{
			base.OnSpawned( newEnemy );
			HandleSpecialtySpawn( newEnemy as TEnemy );
		}

		/// <summary>
		/// This is called after factory creation and <see cref="EnemyController.OnSpawned"/> has been invoked.
		/// </summary>
		protected abstract void HandleSpecialtySpawn( TEnemy newEnemy );
	}
}
