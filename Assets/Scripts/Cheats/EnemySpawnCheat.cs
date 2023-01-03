using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Minipede.Cheats
{
	public class EnemySpawnCheat : EnemyWaveController
	{
		private readonly EnemyWaveController _waveController;

		public EnemySpawnCheat( EnemyWaveController waveController,
			Settings settings, 
			PlayerController playerSpawnController, 
			[Inject( Id = "Main" )] IEnemyWave mainWave, 
			[Inject( Id = "Bonus" )] IEnemyWave[] bonusWaves, 
			SpiderSpawnController spiderSpawnController ) 
			: base( settings, playerSpawnController, mainWave, bonusWaves, spiderSpawnController )
		{
			_waveController = waveController;
		}

		public override void Play()
		{
			Debug.LogWarning( $"<b>{nameof( EnemySpawnCheat )}</b> is enabled. No enemy waves will play." );
		}

		public override void Interrupt()
		{
			Debug.LogWarning( $"<b>{nameof( EnemySpawnCheat )}</b> is enabled. No enemy waves can be interrupted." );
		}
	}
}