namespace Minipede.Gameplay.Enemies.Spawning
{
    public interface IEnemyWave
    {
        event System.Action<IEnemyWave> Completed;

        bool IsRunning { get; }

        void StartSpawning();
        void OnPlayerDied();

        [System.Serializable]
        public struct Settings
		{
            public float StartDelay;
		}
	}
}
