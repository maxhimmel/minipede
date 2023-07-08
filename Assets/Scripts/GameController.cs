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

		private readonly ILevelStartSequence _startSequence;
		private readonly PlayerController _playerSpawnController;
		private readonly LevelGenerator _levelGenerator;
		private readonly DayNightController _dayNightController;
		private readonly MushroomPopulationController _mushroomPopulationController;
		private readonly AudioBankLoader _audioBankLoader;
		private readonly SceneLoader _sceneLoader;

		public GameController( ILevelStartSequence startSequence,
			PlayerController playerSpawnController,
			LevelGenerator levelGenerator,
			DayNightController dayNightController,
			MushroomPopulationController mushroomPopulationController,
			AudioBankLoader audioBankLoader,
			SceneLoader sceneLoader )
		{
			_startSequence = startSequence;
			_playerSpawnController = playerSpawnController;
			_levelGenerator = levelGenerator;
			_dayNightController = dayNightController;
			_mushroomPopulationController = mushroomPopulationController;
			_audioBankLoader = audioBankLoader;
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

			_dayNightController.Play( AppHelper.AppQuittingToken );

			IsReady = true;
		}

		private void OnPlayerDied()
		{
			_mushroomPopulationController.Deactivate();
			_dayNightController.Stop();
		}
	}
}
