using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

		private MenuController _menuController;
		private ScreenFadeController _screenFader;

		[Inject]
		public void Construct( MenuController menuController,
			ScreenFadeController screenFader )
		{
            _menuController = menuController;
			_screenFader = screenFader;

			_startButton.onClick.AddListener( OnStart );
			_settingsButton.onClick.AddListener( _menuController.Open<SettingsMenu> );
			_quitButton.onClick.AddListener( AppHelper.Quit );
		}

		private async void OnStart()
		{
			var activeScene = SceneManager.GetActiveScene();

			int nextSceneIndex = activeScene.buildIndex + 1;
			nextSceneIndex %= SceneManager.sceneCountInBuildSettings;

			await _screenFader.FadeOut( 1 );
			SceneManager.LoadScene( nextSceneIndex );
		}
	}
}
