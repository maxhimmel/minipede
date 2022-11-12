namespace Minipede.Gameplay.Enemies.Spawning
{
    public interface IEnemyWave
    {
        event CompletedSignature Completed;
        delegate void CompletedSignature( IEnemyWave wave, bool isSuccess );

        bool IsRunning { get; }

        void StartSpawning();

        /// <returns>True if the wave was completed or false if it should restart.</returns>
        bool Interrupt();
	}
}
