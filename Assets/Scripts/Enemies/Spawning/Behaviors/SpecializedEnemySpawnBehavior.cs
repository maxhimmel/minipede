namespace Minipede.Gameplay.Enemies.Spawning
{
	public abstract class SpecializedEnemySpawnBehavior<TEnemy> : EnemySpawnBehavior
		where TEnemy : EnemyController
	{
		public override System.Type EnemyType => typeof( TEnemy );

		public override void Perform( EnemyController enemy )
		{
			HandleSpecialtySpawn( enemy as TEnemy );

			base.Perform( enemy );
		}

		/// <summary>
		/// This is called before <see cref="EnemyController.StartMainBehavior"/> has been invoked.
		/// </summary>
		protected abstract void HandleSpecialtySpawn( TEnemy newEnemy );
	}
}
