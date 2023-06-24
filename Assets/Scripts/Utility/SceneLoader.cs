using Cysharp.Threading.Tasks;
using Minipede.Gameplay.UI;
using UnityEngine.SceneManagement;

namespace Minipede.Utility
{
	public class SceneLoader
	{
		public bool IsLoading { get; private set; }
		public Scene Current => SceneManager.GetActiveScene();

		private readonly Settings _settings;
		private readonly ScreenFadeController _screenFader;
		private readonly PauseModel _pauseModel;

		public SceneLoader( Settings settings,
			ScreenFadeController screenFader,
			PauseModel pauseModel )
		{
			_settings = settings;
			_screenFader = screenFader;
			_pauseModel = pauseModel;
		}

		public async UniTask Load( string sceneName, LoadSceneMode mode = LoadSceneMode.Single )
		{
			IsLoading = true;

			await _screenFader.FadeOut( _settings.FadeDuration );
			{
				await SceneManager.LoadSceneAsync( sceneName, mode );

				_pauseModel.Set( false );

				if ( _settings.FadeInDelay > 0 )
				{
					await TaskHelpers.DelaySeconds( _settings.FadeInDelay );
				}
			}
			await _screenFader.FadeIn( _settings.FadeDuration );

			IsLoading = false;
		}

		[System.Serializable]
		public class Settings
		{
			public float FadeDuration;
			public float FadeInDelay;
		}
	}
}