using Cysharp.Threading.Tasks;
using Minipede.Gameplay;
using Minipede.Gameplay.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Minipede.Utility
{
	public class SceneLoader
	{
		public bool IsLoading { get; private set; }
		public Scene Current => SceneManager.GetActiveScene();

		private readonly Settings _settings;
		private readonly ScreenFadeController _screenFader;
		private readonly SignalBus _signalBus;

		public SceneLoader( Settings settings,
			ScreenFadeController screenFader,
			SignalBus signalBus )
		{
			_settings = settings;
			_screenFader = screenFader;
			_signalBus = signalBus;
		}

		public async UniTask Load( string sceneName, LoadSceneMode mode = LoadSceneMode.Single )
		{
			IsLoading = true;

			await _screenFader.FadeOut( _settings.FadeDuration );
			{
				await SceneManager.LoadSceneAsync( sceneName, mode );

				_signalBus.Fire( new PausedSignal( isPaused: false ) );
			}
			await _screenFader.FadeIn( _settings.FadeDuration );

			IsLoading = false;
		}

		[System.Serializable]
		public struct Settings
		{
			public float FadeDuration;
		}
	}
}