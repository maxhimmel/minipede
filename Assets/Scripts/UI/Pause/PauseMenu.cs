using UnityEngine;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
    public class PauseMenu : Menu
    {
        [Header( "Buttons" )]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _settingsButton;

		public override void Initialize()
		{
			base.Initialize();

            _resumeButton.onClick.AddListener( OnResumed );
            _signalBus.Subscribe<PausedSignal>( OnPaused );
            _quitButton.onClick.AddListener( OnQuit );

            _settingsButton.onClick.AddListener( _menuController.Open<SettingsMenu> );
        }

        private void OnResumed()
		{
            _signalBus.Fire( new PausedSignal( isPaused: false ) );
        }

        private void OnPaused( PausedSignal signal )
        {
            if ( signal.IsPaused )
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
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
	}
}
