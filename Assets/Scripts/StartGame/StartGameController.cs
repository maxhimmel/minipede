using System;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Waves;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay
{
	public class StartGameController : ILevelInitializer,
		IInitializable,
		IDisposable
	{
		public bool IsReady { get; private set; }

		private readonly LevelGenerator _levelGenerator;
		private readonly WaveController _waveController;

		public StartGameController( LevelGenerator levelGenerator,
			WaveController waveController )
		{
			_levelGenerator = levelGenerator;
			_waveController = waveController;
		}

		public async void Initialize()
		{
			await _levelGenerator.GenerateLevel().Cancellable( AppHelper.AppQuittingToken );

			_waveController.Play()
				.Cancellable( AppHelper.AppQuittingToken )
				.Forget();

			IsReady = true;
		}

		public void Dispose()
		{
			_waveController.Interrupt();
		}
	}
}
