using Cysharp.Threading.Tasks;
using Minipede.Gameplay.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minipede.Utility
{
	public class SceneLoader
	{
		public bool IsLoading { get; private set; }
		public Scene Current => SceneManager.GetActiveScene();

		private readonly Settings _settings;
		private readonly ScreenFadeController _screenFader;

		public SceneLoader( Settings settings,
			ScreenFadeController screenFader )
		{
			_settings = settings;
			_screenFader = screenFader;
		}

		public async UniTask Load( string sceneName, LoadSceneMode mode = LoadSceneMode.Single )
		{
			IsLoading = true;

			await _screenFader.FadeOut( _settings.FadeDuration );
			{
				await SceneManager.LoadSceneAsync( sceneName, mode );
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