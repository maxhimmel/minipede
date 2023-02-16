using Cysharp.Threading.Tasks;
using Minipede.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class StartMenu : MonoBehaviour
	{
		[SerializeField] private string _entryLevel = "Tester";

		[Space]
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
			_settingsButton.onClick.AddListener( () =>
			{
				_menuController.Open<SettingsMenu>();
				gameObject.SetActive( false );
			} );
			_quitButton.onClick.AddListener( AppHelper.Quit );

			_menuController.Closed += OnMainMenuClosed;
		}

		private void OnStart()
		{
			_menuController.Closed -= OnMainMenuClosed;

			_canvasGroup.interactable = false;

			_sceneLoader.Load( _entryLevel ).Forget();
		}

		private void OnMainMenuClosed()
		{
			gameObject.SetActive( true );
		}
	}
}
