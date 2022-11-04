namespace Minipede.Gameplay.Enemies.Spawning
{
    public interface IEnemyWave
    {
        event System.Action<IEnemyWave> Completed;

        bool IsRunning { get; }

        void StartSpawning();

        /// <returns>True if the wave was completed or false if it should restart.</returns>
        bool Interrupt();
	}
}
