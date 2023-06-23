using System;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Audio;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.StartSequence;
using Minipede.Gameplay.Waves;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay
{
	public class GameController : ILevelInitializer,
		IInitializable, 
		IDisposable
	{
		public bool IsReady { get; private set; }

		private readonly LevelStartSequenceController _startSequence;
		private readonly PlayerController _playerSpawnController;
		private readonly LevelGenerator _levelGenerator;
		private readonly IWaveController _waveController;
		private readonly MushroomPopulationController _mushroomPopulationController;
		private readonly NighttimeController _nighttimeController;
		private readonly AudioBankLoader _audioBankLoader;
		private readonly LevelCycleTimer _levelCycleTimer;
		private readonly SceneLoader _sceneLoader;

		public GameController( LevelStartSequenceController startSequence,
			PlayerController playerSpawnController,
			LevelGenerator levelGenerator,
			IWaveController waveController,
			MushroomPopulationController mushroomPopulationController,
			NighttimeController nighttimeController,
			AudioBankLoader audioBankLoader,
			LevelCycleTimer levelCycleTimer,
			SceneLoader sceneLoader )
		{
			_startSequence = startSequence;
			_playerSpawnController = playerSpawnController;
			_levelGenerator = levelGenerator;
			_waveController = waveController;
			_mushroomPopulationController = mushroomPopulationController;
			_nighttimeController = nighttimeController;
			_audioBankLoader = audioBankLoader;
			_levelCycleTimer = levelCycleTimer;
			_sceneLoader = sceneLoader;
		}

		public void Dispose()
		{
			_audioBankLoader.UnloadBanks().Forget();
			_playerSpawnController.PlayerDied -= OnPlayerDied;
		}

		public async void Initialize()
		{
			_playerSpawnController.PlayerDied += OnPlayerDied;

			await _audioBankLoader.LoadBanks();

			await _levelGenerator.GenerateLevel().Cancellable( AppHelper.AppQuittingToken );
			_startSequence.CreateLighthouseMushrooms();

			await UniTask.WaitWhile( () => _sceneLoader.IsLoading );

			await _startSequence.Play( AppHelper.AppQuittingToken ).SuppressCancellationThrow();

			if ( AppHelper.IsQuitting )
			{
				return;
			}

			//_playerSpawnController.RespawnPlayer();
			_waveController.Play().Forget();
			_levelCycleTimer.Play();

			IsReady = true;
		}

		private void OnPlayerDied()
		{
			_mushroomPopulationController.Deactivate();
			_nighttimeController.Dispose();
			_waveController.Pause();
			_levelCycleTimer.Stop();
		}
	}
}
