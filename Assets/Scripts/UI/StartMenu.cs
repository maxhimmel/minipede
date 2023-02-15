using Cysharp.Threading.Tasks;
using Minipede.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class StartMenu : MonoBehaviour
    {
		[SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

		private MenuController _menuController;
		private SceneLoader _sceneLoader;

		[Inject]
		public void Construct( MenuController menuController,
			SceneLoader sceneLoader )
		{
            _menuController = menuController;
			_sceneLoader = sceneLoader;

			_startButton.onClick.AddListener( OnStart );
			_settingsButton.onClick.AddListener( _menuController.Open<SettingsMenu> );
			_quitButton.onClick.AddListener( AppHelper.Quit );
		}

		private void OnStart()
		{
			_canvasGroup.interactable = false;

			_sceneLoader.Load( "Tester" ).Forget();
		}
	}
}
