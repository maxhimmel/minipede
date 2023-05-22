using Cysharp.Threading.Tasks;
using Minipede.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class PauseMenu : Menu
    {
        [Header( "Buttons" )]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _settingsButton;

        private PauseModel _pauseModel;
        private SceneLoader _sceneLoader;

        [Inject]
		public void Construct( PauseModel pauseModel,
            SceneLoader sceneLoader )
		{
            _pauseModel = pauseModel;
            _sceneLoader = sceneLoader;
		}

		public override void Initialize()
		{
			base.Initialize();

            _resumeButton.onClick.AddListener( OnResumed );
            _pauseModel.Changed += OnPaused;
            _quitButton.onClick.AddListener( OnQuit );

            _settingsButton.onClick.AddListener( _menuController.Open<SettingsMenu> );
        }

        private void OnResumed()
		{
            _pauseModel.Set( false );
        }

        private void OnPaused( PauseModel model )
        {
            if ( model.IsPaused )
			{
                _menuController.Open<PauseMenu>();
			}
            else
			{
                _menuController.Clear();
			}
        }

        private void OnQuit()
		{
            _sceneLoader.Load( "StartMenu" ).Forget();
            _menuController.Clear();
        }
	}
}
